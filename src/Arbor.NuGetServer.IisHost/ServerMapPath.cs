using System.Web;
using System.Web.Http;

using Arbor.NuGetServer.Core;

namespace Arbor.NuGetServer.IisHost
{
    public class ServerMapPath : IPathMapper
    {
        public string MapPath(string relativePath)
        {
            return HttpContext.Current.Server.MapPath(relativePath);
        }
    }
}