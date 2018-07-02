using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Abstractions;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    [UsedImplicitly]
    public sealed class ConfigurationNuGetTenantReadService : INuGetTenantReadService
    {
        private readonly ImmutableArray<NuGetTenantConfiguration> _nugetTenants;

        public ConfigurationNuGetTenantReadService(IReadOnlyCollection<NuGetTenantConfiguration> nugetTenants)
        {
            _nugetTenants = nugetTenants.ToImmutableArray();
        }

        public ImmutableArray<NuGetTenantId> GetNuGetTenantIds()
        {
            return _nugetTenants.Select(item => item.TenantId).ToImmutableArray();
        }

        public ImmutableArray<NuGetTenantConfiguration> GetNuGetTenantConfigurations()
        {
            return _nugetTenants;
        }

        public Task<IReadOnlyList<TenantWebHook>> GetPackageWebHooksAsync(
            NuGetTenantId tenantId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<TenantWebHook>>(ImmutableArray<TenantWebHook>.Empty);
        }
    }
}
