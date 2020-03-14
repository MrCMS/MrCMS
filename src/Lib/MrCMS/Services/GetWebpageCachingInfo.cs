using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Website;
using MrCMS.Website.CMS;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MrCMS.Services
{
    public class GetWebpageCachingInfo : IGetWebpageCachingInfo
    {
        private readonly IQuerySerializer _querySerializer;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IConfigurationProvider _configurationProvider;

        public GetWebpageCachingInfo(IQuerySerializer querySerializer, IGetCurrentUser getCurrentUser,
            IConfigurationProvider configurationProvider)
        {
            _querySerializer = querySerializer;
            _getCurrentUser = getCurrentUser;
            _configurationProvider = configurationProvider;
        }

        public async Task<CachingInfo> Get(Webpage webpage, IQueryCollection queryData)
        {
            var type = webpage?.GetType();
            var attribute = type?.GetCustomAttribute<WebpageOutputCacheableAttribute>(inherit: false);
            if (attribute == null)
            {
                return CachingInfo.DoNotCache;
            }

            var hasPermissions = webpage.HasCustomPermissions &&
                                 webpage.PermissionType == WebpagePermissionType.PasswordBased;
            var settings = await _configurationProvider.GetSiteSettings<PageDefaultsSettings>();
            var shouldCache = !webpage.DoNotCache && _getCurrentUser.Get() == null &&
                              !settings.CacheDisabled(type) && !hasPermissions;

            return new CachingInfo(shouldCache, GetCacheKey(webpage, queryData),
                TimeSpan.FromSeconds(attribute.Length),
                attribute.ExpiryType);
        }

        private string GetCacheKey(Webpage webpage, IQueryCollection queryData)
        {
            var cacheKey = $"Webpage.{webpage.Id}";
            cacheKey = _querySerializer.AppendToUrl(cacheKey,
                queryData.ToDictionary(x => x.Key, x => (object) x.Value));
            return cacheKey;
        }
    }
}