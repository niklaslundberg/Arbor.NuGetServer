using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.WebHooks;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    [UsedImplicitly]
    public sealed class InMemoryNuGetTenantReadService : INuGetTenantReadService
    {
        public ImmutableArray<NuGetTenantId> GetNuGetTenantIds()
        {
            return GetNuGetTenantConfigurations()
                .Select(config => config.TenantId)
                .ToImmutableArray();
        }

        public ImmutableArray<NuGetTenantConfiguration> GetNuGetTenantConfigurations()
        {
            NuGetTenantConfiguration[] tenantsId =
            {
                new NuGetTenantConfiguration(new NuGetTenantId("test"), "testkey", "testuser", "testpassword"),
                new NuGetTenantConfiguration(new NuGetTenantId("test2"), "test2key", "test2user", "test2password"),
                new NuGetTenantConfiguration(new NuGetTenantId("test3"), "test3key", "", "")
            };

            return tenantsId
                .OrderBy(_ => _.TenantId)
                .ToImmutableArray();
        }

        public Task<IReadOnlyList<TenantWebHook>> GetPackageWebHooksAsync(
            NuGetTenantId tenantId,
            CancellationToken cancellationToken)
        {
            var webhooks = new List<TenantWebHook>
            {
                new TenantWebHook(new WebHookConfiguration(new Uri("http://localhost:8042"), "test"),
                    new NuGetTenantId("test")),
                new TenantWebHook(new WebHookConfiguration(new Uri("http://localhost:8042"), "test2"),
                    new NuGetTenantId("test2"))
            };

            IReadOnlyList<TenantWebHook> result = webhooks
                .Where(hook => hook.TenantId == tenantId)
                .ToImmutableArray();

            return Task.FromResult(result);
        }
    }
}
