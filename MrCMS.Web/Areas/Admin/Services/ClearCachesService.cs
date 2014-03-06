using System.Web.Configuration;
using MrCMS.Config;
using MrCMS.DbConfiguration.Caches.Azure;
using MrCMS.Website.Caching;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class ClearCachesService : IClearCachesService
    {
        private readonly ICacheManager _cacheManager;

        public ClearCachesService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public void ClearCache()
        {
            var mrCMSSection = WebConfigurationManager.GetSection("mrcms") as MrCMSConfigSection;
            if (mrCMSSection != null)
            {
                var cacheProvider = mrCMSSection.CacheProvider;
                if (cacheProvider == typeof(AzureCacheProvider))
                {
                    AzureCacheFactory.Instance.GetCache(mrCMSSection.CacheName).Clear();
                    return;
                }
            }
            _cacheManager.Clear();

        }
    }
}