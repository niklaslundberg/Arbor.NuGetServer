using System;
using Arbor.NuGetServer.Abstractions;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.IisHost
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
