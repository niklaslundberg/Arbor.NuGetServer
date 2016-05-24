using System.Data.Services;
using System.ServiceModel.Activation;
using System.Web.Routing;

using Arbor.KVConfiguration.Core;
using Arbor.NuGetServer.Core.Extensions;

using NuGet.Server.DataServices;

namespace Arbor.NuGetServer.IisHost.DataServices
{
    public static class NuGetRoutes
    {
        public static void Configure()
        {
            MapRoutes(RouteTable.Routes);
        }

        private static void MapRoutes(RouteCollection routes)
        {
            const string Key = "nuget:base-route";
            var nugetRoute =
                KVConfigurationManager.AppSettings[Key].ThrowIfNullOrWhitespace(
                    $"AppSetting with key '{Key}' is not set");

            var factory = new DataServiceHostFactory();
            var serviceRoute = new ServiceRoute(nugetRoute, factory, typeof(Packages))
                                   {
                                       Defaults = new RouteValueDictionary
                                                      {
                                                          { "serviceType", "odata" }
                                                      },
                                       Constraints = new RouteValueDictionary
                                                         {
                                                             { "serviceType", "odata" }
                                                         }
                                   };
            routes.Add(nugetRoute, serviceRoute);
        }
    }
}