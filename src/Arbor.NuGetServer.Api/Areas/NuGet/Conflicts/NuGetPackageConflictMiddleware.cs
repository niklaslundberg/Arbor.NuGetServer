using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Api.Areas.Abstraction;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using JetBrains.Annotations;
using Microsoft.Owin;
using NuGet;
using ILogger = Serilog.ILogger;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Conflicts
{
    [UsedImplicitly]
    public class NuGetPackageConflictMiddleware : OwinMiddleware
    {
        private readonly bool _allowOverride;
        private readonly ILogger _logger;
        private readonly string _physicalPath;
        private readonly IRequestFileHelper _requestFileHelper;

        public NuGetPackageConflictMiddleware(
            [NotNull] OwinMiddleware next,
            [NotNull] ILogger logger,
            [NotNull] IKeyValueConfiguration keyValueConfiguration,
            [NotNull] IPathMapper pathMapper,
            [NotNull] IRequestFileHelper requestFileHelper)
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
            _requestFileHelper = requestFileHelper ?? throw new ArgumentNullException(nameof(requestFileHelper));

            string packagesPath = keyValueConfiguration[PackageConfigurationConstants.PackagePath];

            _allowOverride = keyValueConfiguration
                [PackageConfigurationConstants.AllowPackageOverride].ParseAsBoolOrDefault(
                false);

            _physicalPath = pathMapper.MapPath(packagesPath);
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    IReadOnlyList<IHttpPostedFile> postedFiles = _requestFileHelper.GetFiles(context);

                    var packageIdentifiers = new List<PackageIdentifier>();

                    foreach (IHttpPostedFile file in postedFiles)
                    {
                        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.nupkg");
                        file.SaveAs(tempFilePath);

                        var myPackage = new ZipPackage(tempFilePath);

                        string id = myPackage.Id;
                        global::NuGet.Versioning.SemanticVersion semVer =
                            global::NuGet.Versioning.SemanticVersion.Parse(myPackage.Version.ToNormalizedString());

                        if (File.Exists(tempFilePath))
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

                        string normalizedSemVer = semVer.ToNormalizedString();

                        string existingPackage = Path.Combine(
                            _physicalPath,
                            id,
                            normalizedSemVer,
                            $"{id}.{normalizedSemVer}.nupkg");

                        if (!_allowOverride)
                        {
                            if (File.Exists(existingPackage))
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                await
                                    context.Response.WriteAsync(
                                        $"The package '{id}' version '{normalizedSemVer}' already exists");
                                return;
                            }
                        }

                        packageIdentifiers.Add(new PackageIdentifier(id, semVer));
                    }

                    foreach (PackageIdentifier packageIdentifier in packageIdentifiers)
                    {
                        //await Task.Run(() => _mediator.Publish(new PackagePushedNotification(packageIdentifier))); //TODO add tenant support
                    }

                    context.Request.Body.Position = 0;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Could not intercept NuGet PUT request");
                    throw;
                }
            }

            await Next.Invoke(context);
        }
    }
}
