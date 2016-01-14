using System.Data.Services;
using System.ServiceModel.Activation;
using System.Web.Routing;

using NuGet.Server.DataServices;

using WebActivatorEx;

using NuGetRoutes = Arbor.NuGetServer.IisHost.DataServices.NuGetRoutes;

[assembly: PreApplicationStartMethod(typeof(NuGetRoutes), "Start")]

namespace Arbor.NuGetServer.IisHost.DataServices
{
    public static class NuGetRoutes
    {
        public static void Start()
        {
            MapRoutes(RouteTable.Routes);
        }

        private static void MapRoutes(RouteCollection routes)
        {
            // The default route is http://{root}/nuget/Packages
            var factory = new DataServiceHostFactory();
            var serviceRoute = new ServiceRoute("nuget", factory, typeof(Packages));
            serviceRoute.Defaults = new RouteValueDictionary { { "serviceType", "odata" } };
            serviceRoute.Constraints = new RouteValueDictionary { { "serviceType", "odata" } };
            routes.Add("nuget", serviceRoute);
        }
    }
}
