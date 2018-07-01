using System;
using Arbor.NuGetServer.Abstractions;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    [UsedImplicitly]
    public class TenantRouteHelper : ITenantRouteHelper
    {
        private readonly IRouteHelper _routeHelper;

        public TenantRouteHelper([NotNull] IRouteHelper routeHelper)
        {
            _routeHelper = routeHelper ?? throw new ArgumentNullException(nameof(routeHelper));
        }

        public NuGetTenantId GetTenant()
        {
            string routeName = _routeHelper.GetCurrentRouteName();

            if (string.IsNullOrWhiteSpace(routeName))
            {
                return null;
            }

            return new NuGetTenantId(routeName);
        }
    }
}
