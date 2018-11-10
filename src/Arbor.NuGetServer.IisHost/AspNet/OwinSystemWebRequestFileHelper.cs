using System.Collections.Generic;
using System.Linq;
using System.Web;
using Arbor.NuGetServer.Api.Areas.Http;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    [UsedImplicitly]
    public class OwinSystemWebRequestFileHelper : IRequestFileHelper
    {
        public IReadOnlyList<IHttpPostedFile> GetFiles(IOwinContext context)
        {
            HttpContextBase httpContextBase = HttpContextHelper.GetCurrentContext();

            CustomHttpPostedFile[] httpPostedFiles = httpContextBase.Request.Files.OfType<HttpPostedFile>()
                .Select(file => new CustomHttpPostedFile(new HttpPostedFileWrapper(file)))
                .ToArray();

            return httpPostedFiles;
        }
    }
}
