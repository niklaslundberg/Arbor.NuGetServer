namespace Arbor.NuGetServer.Api.Clean
{
    public static class CleanConstants
    {
        public const string Route = "manage/clean";

        public const string CleanEnabled = "nuget:clean:enabled";

        public const string PackagesToKeepKey = "urn:arbor-nuget-server:nuget:clean:packages-to-keep-count";

        public static class DefaultValues
        {
            public const int PackagesToKeep = 5;
        }
    }
}