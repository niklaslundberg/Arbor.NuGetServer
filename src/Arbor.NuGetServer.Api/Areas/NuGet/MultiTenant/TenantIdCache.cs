using System;
using System.Collections.Concurrent;

namespace Arbor.NuGetServer.Api.Areas.NuGet.MultiTenant
{
    internal class TenantIdCache
    {
        private readonly ConcurrentDictionary<Uri, NuGetTenantId> _cache = new ConcurrentDictionary<Uri, NuGetTenantId>();

        public NuGetTenantId Get(Uri uri)
        {
            if (_cache.TryGetValue(uri, out NuGetTenantId nuGetTenantId))
            {
                return nuGetTenantId;
            }

            return null;
        }

        public void Set(NuGetTenantId nuGetTenantId, Uri uri)
        {
            _cache.TryAdd(uri, nuGetTenantId);
        }
    }
}
