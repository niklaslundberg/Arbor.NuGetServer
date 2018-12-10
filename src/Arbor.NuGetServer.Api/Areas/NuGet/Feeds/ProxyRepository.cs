using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arbor.NuGetServer.Api.Areas.CommonExtensions;
using Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant;
using NuGet;
using NuGet.Server.Core.Infrastructure;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public class ProxyRepository : ITenantServerPackageRepository
    {
        private readonly MultiTenantRepository _multiTenantRepository;
        private readonly NuGetCache _nuGetCache;

        public ProxyRepository(MultiTenantRepository multiTenantRepository, NuGetCache nuGetCache)
        {
            _multiTenantRepository = multiTenantRepository;
            _nuGetCache = nuGetCache;
        }

        public async Task AddPackageAsync(IPackage package, CancellationToken token)
        {
            _nuGetCache.Invalidate(_multiTenantRepository.CurrentTenantId.TenantId);
            await _multiTenantRepository.AddPackageAsync(package, token);
        }

        public Task<IEnumerable<IServerPackage>> GetPackagesAsync(
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            return _multiTenantRepository.GetPackagesAsync(compatibility, token);
        }

        public async Task<IEnumerable<IServerPackage>> SearchAsync(
            string searchTerm,
            IEnumerable<string> targetFrameworks,
            bool allowPrereleaseVersions,
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            ImmutableArray<string> frameworks = targetFrameworks.SafeToImmutableArray();
            string term = $"term:{searchTerm}";
            var allFrameworks = string.Join(",", frameworks);
            var serverPackages = _nuGetCache.TryGet<IEnumerable<IServerPackage>>(
                _multiTenantRepository.CurrentTenantId.TenantId,
                term,
                allFrameworks);

            if (serverPackages != null)
            {
                return serverPackages;
            }

            List<IServerPackage> foundPackages = (await _multiTenantRepository.SearchAsync(searchTerm,
                frameworks,
                allowPrereleaseVersions,
                compatibility,
                token)).ToList();

            _nuGetCache.TryAdd(foundPackages,
                _multiTenantRepository.CurrentTenantId.TenantId,
                term,
                allFrameworks);

            return foundPackages;
        }

        public async Task<IEnumerable<IServerPackage>> SearchAsync(
            string searchTerm,
            IEnumerable<string> targetFrameworks,
            bool allowPrereleaseVersions,
            bool allowUnlistedVersions,
            ClientCompatibility compatibility,
            CancellationToken token)
        {
            ImmutableArray<string> frameworks = targetFrameworks.SafeToImmutableArray();
            string term = $"term:{searchTerm}";
            string allFrameworks = string.Join(",", frameworks);
            string preReleaseFlag = allowPrereleaseVersions.ToString();
            string unlistedFlag = allowUnlistedVersions.ToString();
            string compatibilityFlag = compatibility.ToString();

            var serverPackages = _nuGetCache.TryGet<IEnumerable<IServerPackage>>(
                _multiTenantRepository.CurrentTenantId.TenantId,
                term,
                allFrameworks,
                preReleaseFlag,
                unlistedFlag,
                compatibilityFlag);

            if (serverPackages != null)
            {
                return serverPackages;
            }

            List<IServerPackage> foundPackages = (await _multiTenantRepository.SearchAsync(searchTerm,
                frameworks,
                allowPrereleaseVersions,
                allowUnlistedVersions,
                compatibility,
                token)).ToList();

            _nuGetCache.TryAdd(foundPackages,
                _multiTenantRepository.CurrentTenantId.TenantId,
                term,
                allFrameworks,
                preReleaseFlag,
                unlistedFlag,
                compatibilityFlag);

            return foundPackages;
        }

        public Task ClearCacheAsync(CancellationToken token)
        {
            _nuGetCache.Invalidate(_multiTenantRepository.CurrentTenantId.TenantId);
            return _multiTenantRepository.ClearCacheAsync(token);
        }

        public Task RemovePackageAsync(string packageId, SemanticVersion version, CancellationToken token)
        {
            _nuGetCache.Invalidate(_multiTenantRepository.CurrentTenantId.TenantId);
            return _multiTenantRepository.RemovePackageAsync(packageId, version, token);
        }

        public string Source => _multiTenantRepository.Source;
        public NuGetTenantId CurrentTenantId => _multiTenantRepository.CurrentTenantId;
    }
}
