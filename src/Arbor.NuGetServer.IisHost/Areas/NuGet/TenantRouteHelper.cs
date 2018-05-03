using System;
using Arbor.NuGetServer.IisHost.Areas.AspNet;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public class TenantRouteHelper : ITenantRouteHelper
    {
        private readonly IRouteHelper _routeHelper;

        public TenantRouteHelper([NotNull] IRouteHelper routeHelper)
        {
            _routeHelper = routeHelper ?? throw new ArgumentNullException(nameof(routeHelper));
        }

        public NuGetTenant GetTenant()
        {
            string routeName = _routeHelper.GetCurrentRouteName();

            if (string.IsNullOrWhiteSpace(routeName))
            {
                return null;
            }

            return new NuGetTenant(routeName);
        }
    }
}
