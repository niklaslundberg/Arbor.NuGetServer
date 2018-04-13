using System.Web.Hosting;
using Arbor.NuGetServer.Core;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public class ServerMapPath : IPathMapper
    {
        public string MapPath(string relativePath)
        {
            return HostingEnvironment.MapPath(relativePath);
        }
    }
}
