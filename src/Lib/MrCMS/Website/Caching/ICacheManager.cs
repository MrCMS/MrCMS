using System;

namespace MrCMS.Website.Caching
{
    public interface ICacheManager
    {
        T Get<T>(string key);
        T Set<T>(string key, T obj, TimeSpan time, CacheExpiryType cacheExpiryType);
        T GetOrCreate<T>(string key, Func<T> func, TimeSpan time, CacheExpiryType cacheExpiryType);
        void Clear(string prefix = null);
    }
}