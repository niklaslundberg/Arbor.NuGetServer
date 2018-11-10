using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.WebHooks;

namespace Arbor.NuGetServer.Api.Areas.NuGet
{
    public interface INuGetTenantReadService
    {
        ImmutableArray<NuGetTenantId> GetNuGetTenantIds();

        ImmutableArray<NuGetTenantConfiguration> GetNuGetTenantConfigurations();

        Task<IReadOnlyList<TenantWebHook>> GetPackageWebHooksAsync(NuGetTenantId tenantId, CancellationToken cancellationToken);

        Task<NuGetTenantConfiguration> GetNuGetTenantConfigurationAsync(NuGetTenantId nugetTenantId, CancellationToken cancellationToken = default);
    }
}
