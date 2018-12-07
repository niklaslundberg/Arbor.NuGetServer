using System;
using System.Web;
using System.Web.Caching;
using Arbor.NuGetServer.Api.Areas.NuGet.Feeds;
using Autofac;

namespace Arbor.NuGetServer.IisHost.AspNet
{
    public class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CacheWrapper>().SingleInstance().AsImplementedInterfaces();
        }
    }

    public class CacheWrapper : ICache
    {
        public bool TryGetItem(string cacheKey, out object o)
        {
            object tryGetItem = HttpContext.Current?.Cache.Get(cacheKey);

            if (tryGetItem is null)
            {
                o = null;
                return false;
            }

            o = tryGetItem;
            return true;
        }

        public void TryRemove(string key)
        {
            try
            {
                HttpContext.Current?.Cache.Remove(key);
            }
            catch (Exception)
            {
                //
            }
        }

        public void TryAdd<T>(string cacheKey, T item, DateTime expiresUtc) where T : class
        {
            TryRemove(cacheKey);
            HttpContext.Current?.Cache.Add(cacheKey,
                item,
                null,
                expiresUtc,
                TimeSpan.Zero,
                CacheItemPriority.Default,
                null);
        }
    }
}
