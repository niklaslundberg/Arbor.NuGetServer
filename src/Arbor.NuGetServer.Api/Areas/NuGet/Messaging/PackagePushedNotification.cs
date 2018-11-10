using System;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using Arbor.NuGetServer.Api.Areas.WebHooks;
using JetBrains.Annotations;
using MediatR;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Messaging
{
    public class PackagePushedNotification : INotification, IPackageNotification
    {
        public PackagePushedNotification([NotNull] PackageIdentifier packageIdentifier, NuGetTenantId tenantId, FeedId feedId)
        {
            PackageIdentifier = packageIdentifier ?? throw new ArgumentNullException(nameof(packageIdentifier));
            TenantId = tenantId;
            FeedId = feedId;
        }

        public PackageIdentifier PackageIdentifier { get; }
        public NuGetTenantId TenantId { get; }
        public FeedId FeedId { get; }
    }
}
