using System.Data.Services;
using System.ServiceModel.Activation;
using System.Web.Routing;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.IisHost.Extensions;

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
            ConfigurationStartup.Start();
            MapRoutes(RouteTable.Routes);
        }

        private static void MapRoutes(RouteCollection routes)
        {
            const string Key = "nuget:base-route";
            var nugetRoute =
                KVConfigurationManager.AppSettings[Key].ThrowIfNullOrWhitespace(
                    $"AppSetting with key '{Key}' is not set");

            // The default route is http://{root}/nuget/Packages
            var factory = new DataServiceHostFactory();
            var serviceRoute = new ServiceRoute(nugetRoute, factory, typeof(Packages));
            serviceRoute.Defaults = new RouteValueDictionary { { "serviceType", "odata" } };
            serviceRoute.Constraints = new RouteValueDictionary { { "serviceType", "odata" } };
            routes.Add(nugetRoute, serviceRoute);
        }
    }
}
