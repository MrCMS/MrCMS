using System;
using System.Threading.Tasks;

namespace MrCMS.Website.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        T Set<T>(string key, T obj, TimeSpan time, CacheExpiryType cacheExpiryType);
        Task<T> GetOrCreate<T>(string key, Func<Task<T>> func, TimeSpan time, CacheExpiryType cacheExpiryType);
        void Clear(string prefix = null);
    }
}