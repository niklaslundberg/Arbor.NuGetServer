using System;
using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.Owin;

namespace Arbor.NuGetServer.Api.Areas.OwinExtensions
{
    public static class CustomOwinRequestExtensions
    {
        public static bool IsLoginRequest([NotNull] this IOwinRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Uri.PathAndQuery.Split('?')[0].TrimEnd('/').EndsWith("login", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsGetRequest([NotNull] this IOwinRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase);
        }

        public static ImmutableArray<string> GetPathSegments(this IOwinRequest request)
        {
            if (!request.Path.HasValue)
            {
                return ImmutableArray<string>.Empty;
            }

            const char delimiter = '/';

            string[] segments = request.Path.Value.Split(new[]{delimiter}, StringSplitOptions.RemoveEmptyEntries);

            return segments.ToImmutableArray();
        }
    }
}
