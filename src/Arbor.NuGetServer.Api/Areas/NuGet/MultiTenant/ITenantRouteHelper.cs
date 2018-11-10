namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    public interface ITenantRouteHelper
    {
        NuGetTenantId GetTenantId();
    }
}
