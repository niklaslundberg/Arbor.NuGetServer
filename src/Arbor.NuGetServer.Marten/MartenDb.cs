﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.NuGet;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.WebHooks;
using JetBrains.Annotations;
using Marten;

namespace Arbor.NuGetServer.Marten
{
    [UsedImplicitly]
    public class MartenDb : INuGetTenantReadService
    {
        private readonly IDocumentStore _documentStore;

        public MartenDb([NotNull] IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public ImmutableArray<NuGetTenantId> GetNuGetTenantIds()
        {
            using (IDocumentSession session = _documentStore.LightweightSession())
            {
                ImmutableArray<NuGetTenantId> nugetTenantIds = session.Query<NuGetTenantConfiguration>()
                    .Select(configuration => configuration.TenantId).ToList().ToImmutableArray();

                return nugetTenantIds;
            }
        }

        public ImmutableArray<NuGetTenantConfiguration> GetNuGetTenantConfigurations()
        {
            using (IDocumentSession session = _documentStore.LightweightSession())
            {
                ImmutableArray<NuGetTenantConfiguration> tenants =
                    session.Query<NuGetTenantConfiguration>()
                        .ToImmutableArray();

                return tenants;
            }
        }

        public Task<IReadOnlyList<TenantWebHook>> GetPackageWebHooksAsync(
            NuGetTenantId tenantId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<TenantWebHook>>(Array.Empty<TenantWebHook>());
        }

        public async Task<NuGetTenantConfiguration> GetNuGetTenantConfigurationAsync(
            NuGetTenantId nugetTenantId,
            CancellationToken cancellationToken)
        {
            using (IDocumentSession session = _documentStore.LightweightSession())
            {
                NuGetTenantConfiguration tenant =
                    await session.LoadAsync<NuGetTenantConfiguration>(nugetTenantId.TenantId, cancellationToken);

                return tenant;
            }
        }
    }
}
