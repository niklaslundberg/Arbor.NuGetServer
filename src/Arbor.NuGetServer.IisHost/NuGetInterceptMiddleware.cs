using System;
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

                if (!allowOverride)
                {
                    string packagesPath = KVConfigurationManager.AppSettings["packagesPath"];

                    string physicalPath = HttpContext.Current.Server.MapPath(packagesPath);

                    var fileCount = HttpContext.Current.Request.Files.Count;

                    if (fileCount == 1)
                    {
                        var file = HttpContext.Current.Request.Files[0];

                        string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.nupkg");
                        file.SaveAs(tempFilePath);

                        var myPackage = new ZipPackage(tempFilePath);

                        var id = myPackage.Id;
                        var semVer = myPackage.Version;

                        if (File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                        }

                        string normalizedString = semVer.ToNormalizedString();
                        var existingPackage = Path.Combine(
                            physicalPath,
                            id,
                            normalizedString,
                            $"{id}.{normalizedString}.nupkg");

                        if (File.Exists(existingPackage))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                            await context.Response.WriteAsync("The package already exists");
                            return;
                        }
                    }
                }
            }

            await Next.Invoke(context);
        }
    }
}
