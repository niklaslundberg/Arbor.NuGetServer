using System;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost.Areas.Tenants
{
    public class NuGetTenantStatistics
    {
        public NuGetTenantStatistics([NotNull] NuGetTenantId tenantId)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }

        public NuGetTenantId TenantId { get; }
    }
}
