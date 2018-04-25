using System.Web.Mvc;
using System.Web.Routing;

namespace Arbor.NuGetServer.IisHost.Areas.AspNet
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapRoute(
            //        name: "Default",
            //        url: "{controller}/{action}/{id}",
            //        defaults: new {
            //            controller = "Home",
            //            action = "Index",
            //            id = UrlParameter.Optional
            //        }
            //    );

            routes.MapMvcAttributeRoutes();

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        }
    }
}
