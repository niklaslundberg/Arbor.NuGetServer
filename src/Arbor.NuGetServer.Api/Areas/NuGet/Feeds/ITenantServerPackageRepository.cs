using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public interface ITenantServerPackageRepository : IServerPackageRepository
    {
        NuGetTenantId CurrentTenantId { get; }
    }
}