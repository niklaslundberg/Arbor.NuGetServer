using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;

namespace Arbor.NuGetServer.Abstractions
{
    public interface INuGetTenantReadService
    {
        ImmutableArray<NuGetTenantId> GetNuGetTenantIds();

        ImmutableArray<NuGetTenantConfiguration> GetNuGetTenantConfigurations();

        Task<IReadOnlyList<TenantWebHook>> GetPackageWebHooksAsync(NuGetTenantId tenantId, CancellationToken cancellationToken);

        Task<NuGetTenantConfiguration> GetNuGetTenantConfigurationAsync(NuGetTenantId nugetTenantId, CancellationToken cancellationToken = default);
    }
}
