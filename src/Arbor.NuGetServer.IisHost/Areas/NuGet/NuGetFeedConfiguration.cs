using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class NuGetFeedConfiguration
    {
        public NuGetFeedConfiguration(
            string routeName,
            string routeUrl,
            IServerPackageRepository repository,
            string apiKey)
        {
            RouteName = routeName;
            RouteUrl = routeUrl;
            Repository = repository;
            ApiKey = apiKey;
        }

        public string RouteName { get; }

        public string RouteUrl { get; }

        public IServerPackageRepository Repository { get; }

        public string ApiKey { get; }
    }
}
