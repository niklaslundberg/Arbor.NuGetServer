using System.Data.Services;
using System.ServiceModel.Activation;
using System.Web.Routing;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core.Extensions;

using NuGet.Server;
using NuGet.Server.DataServices;
using NuGet.Server.Publishing;

using RouteMagic;

namespace Arbor.NuGetServer.IisHost.DataServices
{
    public static class NuGetRoutes
    {
        private const string OData = "odata";

        private const string ServiceType = "serviceType";

        public static void Configure()
        {
            MapRoutes(RouteTable.Routes);
            ServiceResolver.SetServiceResolver(new DefaultServiceResolver());
        }

        private static void MapRoutes(RouteCollection routes)
        {
            const string Key = "nuget:base-route";
            var nugetRoute =
                StaticKeyValueConfigurationManager.AppSettings[Key].ThrowIfNullOrWhitespace(
                    $"AppSetting with key '{Key}' is not set");

            var factory = new DataServiceHostFactory();

            routes.MapDelegate(
                "CreatePackageNuGet",
                nugetRoute,
                new
                    {
                        httpMethod = new HttpMethodConstraint("PUT")
                    },
                context => CreatePackageService().CreatePackage(context.HttpContext));

            var serviceRoute = new ServiceRoute(nugetRoute, factory, typeof(Packages))
                                   {
                                       Defaults = new RouteValueDictionary
                                                      {
                                                          { ServiceType, OData }
                                                      },
                                       Constraints = new RouteValueDictionary
                                                         {
                                                             { ServiceType, OData }
                                                         }
                                   };
            routes.Add(nugetRoute, serviceRoute);
        }

        private static IPackageService CreatePackageService()
        {
            return ServiceResolver.Resolve<IPackageService>();
        }
    }
}
