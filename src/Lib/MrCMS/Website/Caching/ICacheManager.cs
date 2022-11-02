using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace MrCMS.Website.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        T GetOrCreate<T>(string key, Func<T> func, TimeSpan time, CacheExpiryType cacheExpiryType,
            CacheItemPriority priority = CacheItemPriority.Normal);
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan time, CacheExpiryType cacheExpiryType,
            CacheItemPriority priority = CacheItemPriority.Normal);
        
        Task SetAsync<T>(string key, Func<Task<T>> func, TimeSpan time, CacheExpiryType cacheExpiryType,
            CacheItemPriority priority = CacheItemPriority.Normal);
        void Clear();
        void Clear(string key);
    }
}