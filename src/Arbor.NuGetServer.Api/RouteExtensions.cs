using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api
{
    public static class RouteExtensions
    {
        public static string TenantId([NotNull] this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            string currentRouteName = uri.PathSegments()
                .FirstOrDefault(item => !item.Equals("nuget", StringComparison.OrdinalIgnoreCase));

            return currentRouteName;
        }

        public static IReadOnlyCollection<string> PathSegments([NotNull] this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            string[] pathSegments = uri.PathAndQuery.Split('?')[0]
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            return pathSegments;
        }
    }
}
