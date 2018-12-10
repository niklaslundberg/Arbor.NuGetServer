using System;
using System.Collections.Concurrent;
using System.Linq;
using Arbor.NuGetServer.Api.Areas.Time;
using JetBrains.Annotations;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public class NuGetCache
    {
        private readonly ICache _cache;
        private readonly ICustomClock _customClock;

        private static readonly ConcurrentDictionary<string, DateTime?> _CacheKeys = new ConcurrentDictionary<string, DateTime?>(StringComparer.OrdinalIgnoreCase);

        public NuGetCache([NotNull] ICache cache, ICustomClock customClock)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _customClock = customClock;
        }

        public void Invalidate(string prefix)
        {
            string[] strings = _CacheKeys.Keys.ToArray().Where(key => key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToArray();

            foreach (string key in strings)
            {
                _CacheKeys.TryRemove(key, out DateTime? _);

                _cache.TryRemove(key);
            }
        }

        public T TryGet<T>(string key, params string[] keyArgs) where T: class
        {
            string cacheKey = CacheKey(key, keyArgs);

            if (!_CacheKeys.TryGetValue(cacheKey, out DateTime? expiresUtc))
            {
                return default;
            }

            if (!expiresUtc.HasValue)
            {
                _CacheKeys.TryRemove(cacheKey, out DateTime? _);
                _cache.TryRemove(cacheKey);
                return default;
            }

            if (_customClock.UtcNow().UtcDateTime > expiresUtc.Value)
            {
                _CacheKeys.TryRemove(cacheKey, out DateTime? _);
                _cache.TryRemove(cacheKey);
            }

            if (_cache.TryGetItem(cacheKey, out object cached) && cached is T cachedT)
            {
                return cachedT;
            };

            return default;
        }

        private static string CacheKey(string key, string[] keyArgs)
        {
            string cacheKey = (key + (keyArgs.Length == 0 ? "" : "+" + string.Join("+", keyArgs))).ToUpperInvariant();
            return cacheKey;
        }

        public void TryAdd<T>(T item, string key, params string[] keyArgs) where T: class
        {
            string cacheKey = CacheKey(key, keyArgs);

            DateTime expiresUtc = _customClock.UtcNow().UtcDateTime.AddMinutes(10);

            if (_CacheKeys.ContainsKey(cacheKey))
            {
                _CacheKeys.TryRemove(cacheKey, out DateTime? _);
            }

            _CacheKeys.TryAdd(cacheKey, expiresUtc);

            _cache.TryAdd(cacheKey, item, expiresUtc);
        }
    }
}