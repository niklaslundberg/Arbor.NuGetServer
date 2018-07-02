using System;
using JetBrains.Annotations;
using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public class NuGetFeedConfiguration
    {
        public NuGetFeedConfiguration(
            [NotNull] string id,
            [NotNull] string routeName,
            [NotNull] string routeUrl,
            [NotNull] IServerPackageRepository repository,
            [CanBeNull] string apiKey,
            [NotNull] string packageDirectory)
        {
            if (string.IsNullOrWhiteSpace(routeName))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(routeName));
            }

            if (string.IsNullOrWhiteSpace(routeUrl))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(routeUrl));
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));
            }

            if (string.IsNullOrWhiteSpace(packageDirectory))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(packageDirectory));
            }

            Id = id;
            RouteName = routeName;
            RouteUrl = routeUrl;
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            ApiKey = apiKey;
            PackageDirectory = packageDirectory;
        }

        public string Id { get; }
        public string RouteName { get; }

        public string RouteUrl { get; }

        public IServerPackageRepository Repository { get; }

        public string ApiKey { get; }

        public string PackageDirectory { get; }
    }
}
