using System;
using System.Collections;
using System.Web.Caching;

namespace MrCMS.Website.Caching
{
    public class HttpCache : ICacheWrapper
    {
        private readonly Cache _cache;

        public HttpCache(Cache cache)
        {
            _cache = cache;
        }

        public void Add(string key, object data, DateTime absoluteExpiration, TimeSpan slidingExpiration,
            CacheItemPriority cacheItemPriority)
        {
            _cache.Add(key, data, null, absoluteExpiration, slidingExpiration, cacheItemPriority, null);
        }

        public void Clear()
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _cache.Remove(enumerator.Key.ToString());
            }
        }

        public object this[string key]
        {
            get { return _cache[key]; }
        }
    }
}