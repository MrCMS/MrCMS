using System;
using System.Web.Caching;
using MrCMS.Helpers;

namespace MrCMS.Website.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly Cache _cache;

        public CacheManager(Cache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key, Func<T> func, TimeSpan time)
        {
            var o = _cache[key];

            if (o != null)
                return o.To<T>();

            o = func.Invoke();

            if (o != null)
            {
                _cache.Add(key, o, null, DateTime.MaxValue, time, CacheItemPriority.AboveNormal, null);

                return o.To<T>();
            }
            return (T) (object) null;
        }
    }
}