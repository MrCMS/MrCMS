using System.Collections.Generic;
using System.Web.Configuration;
using MrCMS.Services.Caching;
using MrCMS.Website.Caching;

namespace MrCMS.Website
{
    public class ClearCachesService : IClearCachesService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IEnumerable<IClearCache> _manualCacheClears;

        public ClearCachesService(ICacheManager cacheManager, IEnumerable<IClearCache> manualCacheClears)
        {
            _cacheManager = cacheManager;
            _manualCacheClears = manualCacheClears;
        }

        public void ClearCache()
        {
            _cacheManager.Clear();

            foreach (var cache in _manualCacheClears)
            {
                cache.ClearCache();
            }
        }
    }
}