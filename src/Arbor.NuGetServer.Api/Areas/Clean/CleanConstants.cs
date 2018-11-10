using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;

namespace Arbor.NuGetServer.Api.Areas.Clean
{
    public static class CleanConstants
    {
        public const string CleanOnStartEnabled = "urn:arbor-nuget-server:nuget:clean:clean-bin-files-on-start:enabled";

        public const string TenantRouteParameterName = "tenant";

        public const string PostRoute = "~" + TenantConstants.NuGetBaseRoute + "/{tenant}/clean";

        public const string CleanGetRoute = "~" + TenantConstants.NuGetBaseRoute + "/{tenant}/cleaning";

        public const string CleanGetRouteName = nameof(CleanGetRoute);

        public const string CleanEnabled = "nuget:clean:enabled";

        public const string PackagesToKeepKey = "urn:arbor-nuget-server:nuget:clean:packages-to-keep-count";

        public static string AutomaticCleanIntervalInSeconds =
            "urn:arbor-nuget-server:nuget:clean:automatic-clean:interval-in-seconds";

        public static string AutomaticCleanEnabled = "urn:arbor-nuget-server:nuget:clean:automatic-clean:enabled";

        public static class DefaultValues
        {
            public const int PackagesToKeep = 5;
        }
    }
}
