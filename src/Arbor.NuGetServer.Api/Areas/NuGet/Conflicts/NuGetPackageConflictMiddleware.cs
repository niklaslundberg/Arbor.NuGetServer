using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.Application;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.NuGet.Feeds;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;
using Microsoft.Owin;
using NuGet;
using NuGet.Server.Core;
using File = Alphaleonis.Win32.Filesystem.File;
using ILogger = Serilog.ILogger;
using Path = Alphaleonis.Win32.Filesystem.Path;
using SemanticVersion = NuGet.Versioning.SemanticVersion;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Conflicts
{
    [UsedImplicitly]
    public class NuGetPackageConflictMiddleware : OwinMiddleware
    {
        private readonly bool _allowOverride;

        private readonly IReadOnlyCollection<NuGetFeedConfiguration> _feedConfigurations;
        private readonly ILogger _logger;

        [NotNull]
        private readonly ITenantServerPackageRepository _serverPackageRepository;

        public NuGetPackageConflictMiddleware(
            [NotNull] OwinMiddleware next,
            [NotNull] ILogger logger,
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            [NotNull] IPathMapper pathMapper,
            [NotNull] ITenantServerPackageRepository serverPackageRepository,
            IReadOnlyCollection<NuGetFeedConfiguration> feedConfigurations)
            : base(next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (keyValueConfiguration == null)
            {
                throw new ArgumentNullException(nameof(keyValueConfiguration));
            }

            if (pathMapper == null)
            {
                throw new ArgumentNullException(nameof(pathMapper));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serverPackageRepository = serverPackageRepository ??
                                       throw new ArgumentNullException(nameof(serverPackageRepository));
            _feedConfigurations = feedConfigurations;

            _allowOverride = keyValueConfiguration
                [PackageConfigurationConstants.AllowPackageOverride].ParseAsBoolOrDefault(
                false);
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (!context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                await Next.Invoke(context);
                return;
            }

            NuGetTenantId nuGetTenantId = _serverPackageRepository.CurrentTenantId;

            NuGetFeedConfiguration nuGetFeedConfiguration = _feedConfigurations.SingleOrDefault(s =>
                s.Id.Equals(nuGetTenantId.TenantId, StringComparison.OrdinalIgnoreCase));

            if (nuGetFeedConfiguration is null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync("Could not find nuget feed configuration");
                return;
            }

            using (var cts = new CancellationTokenSource())
            {
                Stopwatch stopwatch = null;

                PackageIdentifier packageIdentifier = null;
                bool? result = null;
                Exception caughtException = null;
                string tempFilePath = null;
                try
                {
                    stopwatch = Stopwatch.StartNew();

                    try
                    {
                        using (HttpContent content = new StreamContent(context.Request.Body))
                        {
                            content.Headers.TryAddWithoutValidation("Content-Type", context.Request.ContentType);

                            tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.nupkg");
                            if (!content.IsMimeMultipartContent("multipart/form-data"))
                            {
                                var provider = new MultipartMemoryStreamProvider();
                                await content.ReadAsMultipartAsync(provider, cts.Token);

                                if (provider.Contents.Count != 1)
                                {
                                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                    await context.Response.WriteAsync("Expected exactly 1 file part in content",
                                        cts.Token);

                                    return;
                                }

                                HttpContent fileContent = provider.Contents.First();

                                using (var fs = new FileStream(tempFilePath,
                                    FileMode.OpenOrCreate,
                                    FileAccess.Write))
                                {
                                    using (Stream stream = await fileContent.ReadAsStreamAsync())
                                    {
                                        await stream.CopyToAsync(fs);
                                    }

                                    await fs.FlushAsync(cts.Token);
                                }
                            }
                        }

                        IPackage package = PackageFactory.Open(tempFilePath);

                        SemanticVersion semanticVersion =
                            SemanticVersion.Parse(package.Version
                                .ToNormalizedString());
                        string normalizedSemVer = semanticVersion.ToNormalizedString();
                        string packageId = package.Id;

                        string existingPackage = Path.Combine(
                            nuGetFeedConfiguration.PackageDirectory,
                            packageId,
                            normalizedSemVer,
                            $"{packageId}.{normalizedSemVer}.nupkg");

                        if (!_allowOverride)
                        {
                            if (File.Exists(existingPackage))
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                await
                                    context.Response.WriteAsync(
                                        $"The package '{packageId}' version '{normalizedSemVer}' already exists",
                                        cts.Token);
                                return;
                            }
                        }

                        await _serverPackageRepository.AddPackageAsync(package, cts.Token);

                        context.Response.StatusCode = (int)HttpStatusCode.Created;

                        packageIdentifier = new PackageIdentifier(packageId, semanticVersion);

                        //await Task.Run(() => _mediator.Publish(new PackagePushedNotification(packageIdentifier))); //TODO add tenant support

                        context.Request.Body.Position = 0;
                    }
                    catch (Exception ex)
                    {
                        caughtException = ex;
                        _logger.Error(ex, "Could not intercept NuGet PUT request");
                        throw;
                    }
                }
                finally
                {
                    if (stopwatch != null)
                    {
                        stopwatch.Stop();

                        HttpStatusCode[] allowed = { HttpStatusCode.OK, HttpStatusCode.Created };
                        if (caughtException != null)
                        {
                            result = false;
                        }
                        else if (allowed.Any(a => a == (HttpStatusCode)context.Response.StatusCode))
                        {
                            result = true;
                        }

                        string resultText = result.HasValue ? result.Value ? "Succeeded" : "Failed" : "Unknown";

                        _logger.Information("Package push result {PackageId} {Version} {Result} took {ElapsedMilliseconds} ms",
                            packageIdentifier?.PackageId,
                            packageIdentifier?.SemanticVersion?.ToNormalizedString(),
                            resultText,
                            stopwatch.Elapsed.TotalMilliseconds.ToString("F1"));
                    }

                    if (!string.IsNullOrWhiteSpace(tempFilePath) && File.Exists(tempFilePath))
                    {
                        try
                        {
                            File.Delete(tempFilePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "Could not delete temp file '{TempFilePath}'", tempFilePath);
                        }
                    }
                }
            }
        }
    }
}
