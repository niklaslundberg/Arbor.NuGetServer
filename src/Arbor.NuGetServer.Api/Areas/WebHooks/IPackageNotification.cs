using Arbor.NuGetServer.Api.Areas.NuGet;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public interface IPackageNotification
    {
        PackageIdentifier PackageIdentifier { get; }

        NuGetTenantId TenantId { get; }

        FeedId FeedId { get; }
    }
}