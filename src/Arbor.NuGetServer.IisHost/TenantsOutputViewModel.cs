using System.Collections.Generic;
using System.Collections.Immutable;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;

namespace Arbor.NuGetServer.IisHost
{
    public class TenantsOutputViewModel
    {
        public TenantsOutputViewModel(ImmutableArray<NuGetTenantStatistics> nuGetTenants)
        {
            NuGetTenants = nuGetTenants.SafeToImmutableArray();
        }

        public IReadOnlyList<NuGetTenantStatistics> NuGetTenants { get; }
    }
}
