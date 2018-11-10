using System;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.Routing
{
    public static class RouteHelper
    {
        public static string WithParameter([NotNull] this string route, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(route))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(route));
            }

            return route.Replace($"{{{key}}}", value);
        }
    }
}
