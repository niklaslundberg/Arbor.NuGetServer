using System.Collections.Generic;
using System.Collections.Immutable;
using Arbor.NuGetServer.Core.Extensions;

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
