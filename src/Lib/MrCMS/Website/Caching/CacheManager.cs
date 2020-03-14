using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace MrCMS.Website.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly IClearableInMemoryCache _cache;

        public CacheManager(IClearableInMemoryCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            return _cache.Get<T>(key);
        }

        public T Set<T>(string key, T obj, TimeSpan time, CacheExpiryType cacheExpiryType)
        {
            var options = new MemoryCacheEntryOptions();
            if (time > TimeSpan.Zero)
            {
                if (cacheExpiryType == CacheExpiryType.Sliding)
                {
                    options.SlidingExpiration = time;
                }
                else if (cacheExpiryType == CacheExpiryType.Absolute)
                {
                    options.AbsoluteExpirationRelativeToNow = time;
                }
            }

            return _cache.Set(key, obj, options);
        }

        public async Task<T> GetOrCreate<T>(string key, Func<Task<T>> func, TimeSpan time, CacheExpiryType cacheExpiryType)
        {
            if (time <= TimeSpan.Zero)
            {
                return await func();
            }

            return await _cache.GetOrCreate(key, async entry =>
            {
                if (cacheExpiryType == CacheExpiryType.Sliding)
                {
                    entry.SlidingExpiration = time;
                }
                else if (cacheExpiryType == CacheExpiryType.Absolute)
                {
                    entry.AbsoluteExpirationRelativeToNow = time;
                }

                var value = await func();
                entry.Value = value;
                return value;
            });
        }

        public void Clear(string prefix = null)
        {
            _cache.Clear(prefix);
        }
    }
}