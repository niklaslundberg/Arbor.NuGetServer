using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;
using JetBrains.Annotations;
using MediatR;
using Microsoft.Owin;
using NuGet;
using ILogger = Serilog.ILogger;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class NuGetInterceptMiddleware : OwinMiddleware
    {
        private readonly ILogger _logger;
        private bool _allowOverride;
        private IMediator _mediator;
        private string _physicalPath;

        public NuGetInterceptMiddleware(
            OwinMiddleware next,
            ILogger logger,
            IKeyValueConfiguration keyValueConfiguration,
            IMediator mediator)
            : base(next)
        {
            _logger = logger;
            _mediator = mediator;
            IKeyValueConfiguration keyValueConfiguration1 = keyValueConfiguration;

            string packagesPath = keyValueConfiguration1[PackageConfigurationConstants.PackagePath];

            _allowOverride = keyValueConfiguration1
                [PackageConfigurationConstants.AllowPackageOverride].ParseAsBoolOrDefault(
                false);

            _physicalPath = HostingEnvironment.MapPath(packagesPath);
        }

        public override async Task Invoke(IOwinContext context)
        {
            HttpContext httpContext = HttpContext.Current;

            if (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase) && httpContext != null)
            {
                try
                {
                    int fileCount = httpContext.Request.Files.Count;

                    var packageIdentifiers = new List<PackageIdentifier>();

                    for (int fileCounter = 0; fileCounter < fileCount; fileCounter++)
                    {
                        HttpPostedFile file = httpContext.Request.Files[fileCounter];

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
                        await Task.Run(() => _mediator.Publish(new PackagePushedNotification(packageIdentifier)));
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
