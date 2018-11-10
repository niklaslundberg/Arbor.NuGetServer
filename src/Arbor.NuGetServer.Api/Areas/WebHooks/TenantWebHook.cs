using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public class TenantWebHook
    {
        public TenantWebHook(WebHookConfiguration configuration, NuGetTenantId tenantId)
        {
            Configuration = configuration;
            TenantId = tenantId;
        }

        public WebHookConfiguration Configuration { get; private set; }

        public NuGetTenantId TenantId { get; private set; }
    }
}
