namespace Arbor.NuGetServer.Api.Clean
{
    public static class CleanConstants
    {
        public const string CleanOnStartEnabled = "urn:arbor-nuget-server:nuget:clean:clean-bin-files-on-start:enabled";

        public const string PostRoute = "manage/clean";

        public const string GetRoute = "manage/cleaning";

        public const string CleanEnabled = "nuget:clean:enabled";

        public const string PackagesToKeepKey = "urn:arbor-nuget-server:nuget:clean:packages-to-keep-count";

        public static class DefaultValues
        {
            public const int PackagesToKeep = 5;
        }
    }
}
