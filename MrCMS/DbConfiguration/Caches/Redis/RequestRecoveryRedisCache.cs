using System.Collections.Generic;
using System.Web;
using NHibernate.Caches.Redis;
using StackExchange.Redis;

namespace MrCMS.DbConfiguration.Caches.Redis
{
    public class RequestRecoveryRedisCache : RedisCache
    {
        public const string SkipNHibernateCacheKey = "__SkipNHibernateCache__";

        public RequestRecoveryRedisCache(string regionName, IDictionary<string, string> properties, RedisCacheElement element, ConnectionMultiplexer connectionMultiplexer, RedisCacheProviderOptions options)
            : base(regionName, properties, element, connectionMultiplexer, options)
        {

        }

        public override object Get(object key)
        {
            if (HasFailedForThisHttpRequest()) return null;
            return base.Get(key);
        }

        public override void Put(object key, object value)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Put(key, value);
        }

        public override void Remove(object key)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Remove(key);
        }

        public override void Clear()
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Clear();
        }

        public override void Destroy()
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Destroy();
        }

        public override void Lock(object key)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Lock(key);
        }

        public override void Unlock(object key)
        {
            if (HasFailedForThisHttpRequest()) return;
            base.Unlock(key);
        }

        private bool HasFailedForThisHttpRequest()
        {
            var httpContext = HttpContext.Current;
            // we'll be optimistic if it's outside of httpcontext
            if (httpContext == null)
                return false;
            return httpContext.Items.Contains(SkipNHibernateCacheKey);
        }
    }
}