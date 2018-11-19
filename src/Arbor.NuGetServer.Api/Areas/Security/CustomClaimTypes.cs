namespace Arbor.NuGetServer.Api.Areas.Security
{
    public static class CustomClaimTypes
    {
        public const string CanIssueToken = "urn:arbor-nuget-server:claims:can-issue-token";

        public const string PackageIdentifier = "packageid";

        public const string TenantId = "tenant";
    }
}
