using System;
using System.Collections;
using System.Web.Caching;
using MrCMS.Helpers;

namespace MrCMS.Website.Caching
{
    public class CacheManager : ICacheManager
    {
        public const string InternalCachePrefix = "MrCMS.Cache.";
        private readonly Cache _cache;

        public CacheManager(Cache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key, Func<T> func, TimeSpan time, CacheExpiryType cacheExpiryType)
        {
            key = InternalCachePrefix + key;
            object o = null;
            if (time > TimeSpan.Zero)
                o = _cache[key];

            if (o != null)
                return o.To<T>();

            o = func.Invoke();

            if (o != null)
            {
                if (time > TimeSpan.Zero)
                {
                    var absoluteExpiration = cacheExpiryType == CacheExpiryType.Absolute
                        ? DateTime.UtcNow.Add(time)
                        : Cache.NoAbsoluteExpiration;
                    var slidingExpiration = cacheExpiryType == CacheExpiryType.Sliding
                        ? (time)
                        : Cache.NoSlidingExpiration;
                    _cache.Add(key, o, null, absoluteExpiration, slidingExpiration, CacheItemPriority.AboveNormal, null);
                }
                return o.To<T>();
            }
            return (T)(object)null;
        }

        public void Clear()
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _cache.Remove(enumerator.Key.ToString());
            }
        }
    }

    public enum CacheExpiryType
    {
        Sliding,
        Absolute
    }
}