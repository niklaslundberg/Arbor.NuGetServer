using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core;
using Arbor.NuGetServer.Core.Extensions;

using Microsoft.Owin;

using NuGet;

using SemanticVersion = NuGet.Versioning.SemanticVersion;

namespace Arbor.NuGetServer.IisHost.Middleware
{
    public class NuGetInterceptMiddleware : OwinMiddleware
    {
        public NuGetInterceptMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase) && HttpContext.Current != null)
            {
                try
                {
                    var allowOverride =
                        KVConfigurationManager.AppSettings["allowOverrideExistingPackageOnPush"].ParseAsBoolOrDefault(
                            false);

                    string packagesPath = KVConfigurationManager.AppSettings["packagesPath"];

                    string physicalPath = HttpContext.Current.Server.MapPath(packagesPath);

                    int fileCount = HttpContext.Current.Request.Files.Count;

                    List<PackageIdentifier> packageIdentifiers = new List<PackageIdentifier>();

                    for (int fileCounter = 0; fileCounter < fileCount; fileCounter++)
                    {
                        HttpPostedFile file = HttpContext.Current.Request.Files[fileCounter];

                        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.nupkg");
                        file.SaveAs(tempFilePath);

                        var myPackage = new ZipPackage(tempFilePath);

                        string id = myPackage.Id;
                        SemanticVersion semVer = SemanticVersion.Parse(myPackage.Version.ToNormalizedString());

                        if (File.Exists(tempFilePath))
                        {
                            try
                            {
                                File.Delete(tempFilePath);
                            }
                            catch (Exception ex)
                            {
                                Logger.Error($"Could not delete temp file '{tempFilePath}', {ex}");
                            }
                        }

                        string normalizedSemVer = semVer.ToNormalizedString();

                        string existingPackage = Path.Combine(
                            physicalPath,
                            id,
                            normalizedSemVer,
                            $"{id}.{normalizedSemVer}.nupkg");

                        if (!allowOverride)
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

                        context.Set("urn:arbor:nuget:packages", packageIdentifiers);
                    }

                    context.Request.Body.Position = 0;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    throw;
                }
            }

            await Next.Invoke(context);
        }
    }
}