using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NuGet;
using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    public class MultiTenantRepository : IServerPackageRepository
    {
        private readonly IReadOnlyDictionary<NuGetTenantId, IServerPackageRepository> _repositories;
        private readonly ITenantRouteHelper _tenantRouteHelper;

        public MultiTenantRepository(
            [NotNull] Dictionary<NuGetTenantId, IServerPackageRepository> repositories,
            [NotNull] ITenantRouteHelper tenantRouteHelper)
        {
            _repositories = repositories ?? throw new ArgumentNullException(nameof(repositories));
            _tenantRouteHelper = tenantRouteHelper ?? throw new ArgumentNullException(nameof(tenantRouteHelper));
        }

        public Task AddPackageAsync(IPackage package, CancellationToken token)
        {
            return GetTenantRepository().AddPackageAsync(package, token);
        }

        public Task<IEnumerable<IServerPackage>> GetPackagesAsync(
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            return GetTenantRepository().GetPackagesAsync(compatibility, token);
        }

        public Task<IEnumerable<IServerPackage>> SearchAsync(
            string searchTerm,
            IEnumerable<string> targetFrameworks,
            bool allowPrereleaseVersions,
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            return GetTenantRepository()
                .SearchAsync(searchTerm, targetFrameworks, allowPrereleaseVersions, compatibility, token);
        }

        public Task<IEnumerable<IServerPackage>> SearchAsync(
            string searchTerm,
            IEnumerable<string> targetFrameworks,
            bool allowPreReleaseVersions,
            bool allowUnlistedVersions,
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            return GetTenantRepository().SearchAsync(searchTerm,
                targetFrameworks,
                allowPreReleaseVersions,
                allowUnlistedVersions,
                compatibility,
                token);
        }

        public Task ClearCacheAsync(CancellationToken token)
        {
            return GetTenantRepository().ClearCacheAsync(token);
        }

        public Task RemovePackageAsync(string packageId, SemanticVersion version, CancellationToken token)
        {
            return GetTenantRepository().RemovePackageAsync(packageId, version, token);
        }

        public string Source => GetTenantRepository().Source;

        private IServerPackageRepository GetTenantRepository()
        {
            NuGetTenantId nuGetTenantId = _tenantRouteHelper.GetTenantId();

            if (nuGetTenantId is null)
            {
                throw new InvalidOperationException("Could not find tenant for route");
            }

            if (!_repositories.TryGetValue(nuGetTenantId, out IServerPackageRepository repository))
            {
                throw new InvalidOperationException($"Could not find package repository tenant {nuGetTenantId}");
            }

            if (nuGetTenantId is null)
            {
                throw new InvalidOperationException("Could not find package repository for route");
            }

            return repository;
        }
    }
}
