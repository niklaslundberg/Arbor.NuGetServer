using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    [UsedImplicitly]
    public sealed class InMemoryNuGetTenantReadService : INuGetTenantReadService
    {
        public ImmutableArray<NuGetTenant> GetNuGetTenants()
        {
            NuGetTenant[] tenants = { new NuGetTenant("test"), new NuGetTenant("test2") };

            return tenants.ToImmutableArray();
        }
    }
}