using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet;
using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.IisHost.Areas.NuGet
{
    public class MultiTenantRepository : IServerPackageRepository
    {
        private readonly IReadOnlyDictionary<NuGetTenant, IServerPackageRepository> _repositories;
        private readonly ITenantRouteHelper _tenantRouteHelper;

        public MultiTenantRepository(Dictionary<NuGetTenant, IServerPackageRepository> repositories, ITenantRouteHelper tenantRouteHelper)
        {
            _repositories = repositories;
            _tenantRouteHelper = tenantRouteHelper;
        }

        private IServerPackageRepository GetTenantRepository()
        {
            NuGetTenant nuGetTenant = _tenantRouteHelper.GetTenant();

            if (nuGetTenant is null)
            {
                throw new InvalidOperationException("Could not find tenant for route");
            }

            if (!_repositories.TryGetValue(nuGetTenant, out IServerPackageRepository repository))
            {
                throw new InvalidOperationException($"Could not find package repository tenant {nuGetTenant}");
            }


            if (nuGetTenant is null)
            {
                throw new InvalidOperationException("Could not find package repository for route");
            }

            return repository;
        }

        public Task AddPackageAsync(IPackage package, CancellationToken token)
        {
            return GetTenantRepository().AddPackageAsync(package, token);
        }

        public Task<IEnumerable<IServerPackage>> GetPackagesAsync(ClientCompatibility compatibility, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IServerPackage>> SearchAsync(
            string searchTerm,
            IEnumerable<string> targetFrameworks,
            bool allowPrereleaseVersions,
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            return GetTenantRepository().SearchAsync(searchTerm, targetFrameworks, allowPrereleaseVersions, compatibility, token);
        }

        public Task<IEnumerable<IServerPackage>> SearchAsync(
            string searchTerm,
            IEnumerable<string> targetFrameworks,
            bool allowPrereleaseVersions,
            bool allowUnlistedVersions,
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            return GetTenantRepository().SearchAsync(searchTerm, targetFrameworks, allowPrereleaseVersions, allowUnlistedVersions, compatibility, token);
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
    }
}
