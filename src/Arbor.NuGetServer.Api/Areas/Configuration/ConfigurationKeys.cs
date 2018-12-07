﻿namespace Arbor.NuGetServer.Api.Areas.Configuration
{
    public static class ConfigurationKeys
    {
        public const string LoginEnabled = "urn:arbor:nugetserver:login:enabled";

        public const string ApiKey = "apiKey";

        public const string ConflictMiddlewareEnabled = "nuget:custom-conflict-middleware:enabled";

        public const string NuGetWebHookTimeout = "nuget:push:timeout-in-seconds";

        public const string PackagesDirectoryPath = "packagesPath";

        public const string CustomTenantPackagePathsEnabled =
            "urn:arbor:nugetserver:custom-tenant-package-paths:enabled";

        public const string WebHooksEnabled = "urn:arbor:nugetserver:webhooks:enabled";
    }
}
