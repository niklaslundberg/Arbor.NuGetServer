namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    public static class TenantRouteConstants
    {
        public const string TenantsHttpGetRoute = "~/tenants";

        public const string TenantsHttpGetRouteName = nameof(TenantsHttpGetRoute);

        public const string TenantHttpGetRoute = "~/nuget/{tenant}/tenant";

        public const string TenantHttpGetRouteName = nameof(TenantHttpGetRoute);
    }
}
