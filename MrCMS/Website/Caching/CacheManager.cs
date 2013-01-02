using System;
using System.Runtime.Caching;
using NHibernate.Linq;

namespace MrCMS.Website.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly ObjectCache _objectCache;

        public CacheManager(ObjectCache objectCache)
        {
            _objectCache = objectCache;
        }

        public T Get<T>(string key, Func<T> func, TimeSpan time)
        {
            var o = _objectCache[key];

            if (o != null)
                return o.As<T>();

            o = func.Invoke();

            _objectCache.Add(key, o, new CacheItemPolicy
                                         {
                                             SlidingExpiration = time,
                                             AbsoluteExpiration = DateTimeOffset.MaxValue,
                                             Priority = CacheItemPriority.NotRemovable
                                         });

            return o.As<T>();
        }
    }
}