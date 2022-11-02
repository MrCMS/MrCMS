using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace MrCMS.Website.Caching
{
    public abstract class CacheManagerBase<TMemoryCache> where TMemoryCache : IMemoryCache
    {
        protected readonly TMemoryCache Cache;

        public CacheManagerBase(TMemoryCache cache)
        {
            Cache = cache;
        }
        

        public T Get<T>(string key)
        {
            return Cache.Get<T>(key);
        }

        public T GetOrCreate<T>(string key, Func<T> func, TimeSpan time, CacheExpiryType cacheExpiryType,
            CacheItemPriority priority = CacheItemPriority.Normal)
        {
            if (time <= TimeSpan.Zero || string.IsNullOrWhiteSpace(key))
            {
                return func();
            }

            return Cache.GetOrCreate(key, entry =>
            {
                var value = func();
                switch (cacheExpiryType)
                {
                    case CacheExpiryType.Sliding:
                        entry.SlidingExpiration = time;
                        break;
                    case CacheExpiryType.Absolute:
                        entry.AbsoluteExpirationRelativeToNow = time;
                        break;
                }

                entry.Value = value;
                entry.SetPriority(priority);
                return (T) entry.Value;
            });
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan time,
            CacheExpiryType cacheExpiryType, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            if (time <= TimeSpan.Zero || string.IsNullOrWhiteSpace(key))
            {
                return await func();
            }

            return await Cache.GetOrCreateAsync(key, async entry =>
            {
                var value = await func();
                switch (cacheExpiryType)
                {
                    case CacheExpiryType.Sliding:
                        entry.SlidingExpiration = time;
                        break;
                    case CacheExpiryType.Absolute:
                        entry.AbsoluteExpirationRelativeToNow = time;
                        break;
                }

                entry.Value = value;
                entry.SetPriority(priority);
                return (T) entry.Value;
            });
        }

        public async Task SetAsync<T>(string key, Func<Task<T>> func, TimeSpan time, CacheExpiryType cacheExpiryType,
            CacheItemPriority priority = CacheItemPriority.Normal)
        {
            var value = await func();
            using var entry = Cache.CreateEntry(key);
            switch (cacheExpiryType)
            {
                case CacheExpiryType.Sliding:
                    entry.SetSlidingExpiration(time);
                    break;
                case CacheExpiryType.Absolute:
                    entry.SetAbsoluteExpiration(time);
                    break;
            }

            entry.Value = value;
            entry.SetPriority(priority);
        }
    }
}