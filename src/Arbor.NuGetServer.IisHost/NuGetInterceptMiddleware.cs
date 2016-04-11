using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebPages;

using Arbor.KVConfiguration.Core;

using Microsoft.Owin;

using NuGet;

namespace Arbor.NuGetServer.IisHost
{
    public class NuGetInterceptMiddleware : OwinMiddleware
    {
        public NuGetInterceptMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Method.Equals("PUT", StringComparison.OrdinalIgnoreCase))
            {
                var allowOverride =
                    KVConfigurationManager.AppSettings["allowOverrideExistingPackageOnPush"].AsBool(false);

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
                    SemanticVersion semVer = myPackage.Version;

                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
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
            }

            await Next.Invoke(context);
        }
    }
}
