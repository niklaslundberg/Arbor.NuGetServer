namespace Arbor.NuGetServer.Abstractions
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
