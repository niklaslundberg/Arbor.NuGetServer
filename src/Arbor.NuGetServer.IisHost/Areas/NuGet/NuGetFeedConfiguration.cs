using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class NuGetFeedConfiguration
    {
        public NuGetFeedConfiguration(
            string routeName,
            string routeUrl,
            string controllerName,
            IServerPackageRepository repository,
            string apiKey)
        {
            RouteName = routeName;
            RouteUrl = routeUrl;
            ControllerName = controllerName;
            Repository = repository;
            ApiKey = apiKey;
        }

        public string RouteName { get; }

        public string RouteUrl { get; }

        public string ControllerName { get; }

        public IServerPackageRepository Repository { get; }

        public string ApiKey { get; }
    }
}
