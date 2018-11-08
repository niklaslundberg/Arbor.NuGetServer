using Arbor.NuGetServer.Abstractions;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Clean
{
    public static class RouteConstants
    {
        public const string PackageRoute = "~" + TenantConstants.NuGetBaseRoute + "/{tenant}/packages";

        public const string PackageRouteName = nameof(PackageRoute);
    }
}
