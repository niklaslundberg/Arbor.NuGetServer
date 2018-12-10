using System;

namespace Arbor.NuGetServer.Api.Areas.NuGet.Feeds
{
    public interface ICache
    {
        bool TryGetItem(string cacheKey, out object o);

        void TryRemove(string key);

        void TryAdd<T>(string cacheKey, T item, DateTime expiresUtc) where T : class;
    }
}