using System.Web;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.IisHost.AspNet;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    [UsedImplicitly]
    public class TenantRouteHelper : ITenantRouteHelper
    {
        public NuGetTenantId GetTenantId()
        {
            HttpContextBase httpContextWrapper = HttpContextHelper.GetCurrentContext();

            return httpContextWrapper?.GetOwinContext().GetTenantId();
        }
    }
}
