using System.Web.Mvc;
using System.Web.Routing;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapMvcAttributeRoutes();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}
