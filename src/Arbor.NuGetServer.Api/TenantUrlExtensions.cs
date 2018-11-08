using System;
using System.Collections.Immutable;
using Arbor.NuGetServer.Abstractions;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api
{
    public static class TenantUrlExtensions
    {
        public static string GetTenantIdFromUrl([NotNull] this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            ImmutableArray<string> pathSegments = uri.PathSegments();

            if (pathSegments.IsDefaultOrEmpty || pathSegments.Length == 1)
            {
                return null;
            }

            if (!pathSegments[0].Equals("nuget", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return pathSegments[1];
        }

        private static ImmutableArray<string> PathSegments([NotNull] this Uri uri)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (!uri.PathAndQuery.StartsWith(TenantConstants.NuGetBaseRoute, StringComparison.OrdinalIgnoreCase))
            {
                return ImmutableArray<string>.Empty;
            }

            ImmutableArray<string> pathSegments = uri.PathAndQuery.Split('?')[0]
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .ToImmutableArray();

            return pathSegments;
        }
    }
}
