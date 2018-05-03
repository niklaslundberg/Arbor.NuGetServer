using System.Collections.Generic;
using System.Linq;
using System.Web;
using Arbor.NuGetServer.IisHost.Areas.Abstraction;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    [UsedImplicitly]
    public class OwinSystemWebRequestFileHelper : IRequestFileHelper
    {
        public IReadOnlyList<IHttpPostedFile> GetFiles(IOwinContext context)
        {
            HttpContextBase httpContextBase = HttpContextHelper.GetCurrentContext();

            return httpContextBase.Request.Files.OfType<HttpPostedFile>()
                .Select(file => new CustomHttpPostedFile(new HttpPostedFileWrapper(file))).ToArray();
        }
    }
}
