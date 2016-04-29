using System.Collections.Generic;
using System.Web;
using NHibernate.Caches.Redis;
using StackExchange.Redis;

namespace MrCMS.DbConfiguration.Caches.Redis
{
    public class RequestRecoveryRedisCacheProvider : RedisCacheProvider
    {
        protected override RedisCache BuildCache(string regionName, IDictionary<string, string> properties, RedisCacheElement configElement, ConnectionMultiplexer connectionMultiplexer, RedisCacheProviderOptions options)
        {
            options.OnException = (e) =>
            {
                var httpContext = HttpContext.Current;
                if (httpContext != null)
                    httpContext.Items[RequestRecoveryRedisCache.SkipNHibernateCacheKey] = true;
            };

            return new RequestRecoveryRedisCache(regionName, properties, configElement, connectionMultiplexer, options);
        }
    }
}