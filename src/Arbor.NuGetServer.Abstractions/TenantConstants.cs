namespace Arbor.NuGetServer.Abstractions
{
    public static class TenantConstants
    {
        public const string NuGetBaseRoute = "/nuget";

        public const string Tenant = "urn:arbor:nugetserver:tenants:tenant";

        public const string ConfigurationSourceEnabled = "urn:arbor:nugetserver:tenants:configuration-source:enabled";

        public const string InMemorySourceEnabled = "urn:arbor:nugetserver:tenants:in-memory-source:enabled";
    }
}
