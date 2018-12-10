namespace Arbor.NuGetServer.Api.Areas.Configuration
{
    public static class ConfigurationKeys
    {
        public const string SeqUrl = "urn:arbor:nugetserver:logging:seq-url";

        public const string SerilogLogLevel = "urn:arbor:nugetserver:logging:log-level";

        public const string LoginEnabled = "urn:arbor:nugetserver:login:enabled";

        public const string ConflictMiddlewareEnabled = "urn:arbor:nugetserver:custom-conflict-middleware:enabled";

        public const string NuGetWebHookTimeout = "urn:arbor:nugetserver:web-hooks:push:timeout-in-seconds";

        public const string PackagesDirectoryPathKey = "packagesPath";

        public const string CustomTenantPackagePathsEnabled =
            "urn:arbor:nugetserver:custom-tenant-package-paths:enabled";

        public const string WebHooksEnabled = "urn:arbor:nugetserver:web-hooks:enabled";
    }
}
