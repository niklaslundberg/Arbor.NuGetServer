using System;
using System.Security.Claims;

namespace Arbor.NuGetServer.Abstractions
{
    public class NuGetClaimType
    {
        public string Key { get; }

        private NuGetClaimType(string key)
        {
            Key = key;
        }

        public static bool IsNuGetClaimType(Claim claim)
        {
            return claim.Type.StartsWith("urn:arbor-nuget-server:claims:", StringComparison.OrdinalIgnoreCase);
        }

        public static readonly NuGetClaimType CanReadTenantFeed = new NuGetClaimType("urn:arbor-nuget-server:claims:can-read-tenant-feed");

        public static readonly NuGetClaimType CanPushPackage = new NuGetClaimType("urn:arbor-nuget-server:claims:can-push-package");
    }
}
