using System;
using System.Collections;
using System.Web.Caching;
using MrCMS.Helpers;

namespace MrCMS.Website.Caching
{
    public class CacheManager : ICacheManager
    {
        public const string InternalCachePrefix = "MrCMS.Cache.";
        private readonly ICacheWrapper _cache;

        public CacheManager(ICacheWrapper cache)
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
                    _cache.Add(key, o, absoluteExpiration, slidingExpiration, CacheItemPriority.AboveNormal);
                }
                return o.To<T>();
            }
            return (T)(object)null;
        }

        public void Clear(string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                _cache.Clear();
            else
            {
                IDictionaryEnumerator enumerator = _cache.GetEnumerator();
                var fullPrefix = InternalCachePrefix + prefix;
                while (enumerator.MoveNext())
                {
                    var key = enumerator.Key.ToString();
                    if (key.StartsWith(fullPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        _cache.Remove(key);
                    }
                }
            }
        }
    }
}