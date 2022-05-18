using System;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;

namespace MrCMS.Website.Caching
{
    public class CacheManager : CacheManagerBase<IStandardPriorityMemoryCache>, ICacheManager
    {
        private readonly IServiceProvider _serviceProvider;

        public CacheManager(IStandardPriorityMemoryCache cache, IServiceProvider serviceProvider) : base(cache)
        {
            _serviceProvider = serviceProvider;
        }

        //

        public void Clear()
        {
            Cache.Compact(100);
            var factory = _serviceProvider.GetService<ISessionFactory>();
            if (factory != null)
                foreach (var classMetadata in factory.GetAllClassMetadata().Values)
                {
                    factory.EvictEntity(classMetadata.EntityName);
                }
        }
        
        public void Clear(string key)
        {
            Cache.Remove(key);
        }
    }
}