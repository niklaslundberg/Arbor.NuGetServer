namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class NuGetTenant
    {
        public string TenantId { get; }

        public NuGetTenant(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}