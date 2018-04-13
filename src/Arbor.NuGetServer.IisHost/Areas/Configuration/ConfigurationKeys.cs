namespace Arbor.NuGetServer.IisHost.Areas.Configuration
{
    public static class ConfigurationKeys
    {
        public const string ApiKey = "apiKey";

        public const string ConflictMiddlewareEnabled = "nuget:custom-conflict-middleware:enabled";

        public const string NuGetWebHookTimeout = "nuget:push:timeout-in-seconds";

        public const string PackagesDirectoryPath = "packagesPath";
    }
}
