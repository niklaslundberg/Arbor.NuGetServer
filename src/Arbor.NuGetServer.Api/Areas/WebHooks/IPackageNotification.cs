using Arbor.NuGetServer.Abstractions;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Core;

namespace Arbor.NuGetServer.Api.Areas.WebHooks
{
    public interface IPackageNotification
    {
        PackageIdentifier PackageIdentifier { get; }

        NuGetTenantId TenantId { get; }

        FeedId FeedId { get; }
    }
}