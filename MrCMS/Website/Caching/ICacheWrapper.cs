using System;
using System.Collections;
using System.Web.Caching;

namespace MrCMS.Website.Caching
{
    public interface ICacheWrapper
    {
        void Add(string key, object data, DateTime absoluteExpiration, TimeSpan slidingExpiration,
            CacheItemPriority cacheItemPriority);

        void Clear();
        object this[string key] { get; }
        IDictionaryEnumerator GetEnumerator();

        void Remove(string key);
    }
}