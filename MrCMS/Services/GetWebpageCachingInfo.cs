using System;
using System.Reflection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Settings;
using MrCMS.Website;
using StackExchange.Profiling;

namespace MrCMS.Services
{
    public class GetWebpageCachingInfo : IGetWebpageCachingInfo
    {
        private readonly IQuerySerializer _querySerializer;
        private readonly PageDefaultsSettings _settings;

        public GetWebpageCachingInfo(IQuerySerializer querySerializer, PageDefaultsSettings settings)
        {
            _querySerializer = querySerializer;
            _settings = settings;
        }

        public CachingInfo Get(Webpage webpage, object queryData = null)
        {
            using (MiniProfiler.Current.Step("Get caching info"))
            {
                var type = webpage?.GetType();
                var attribute = type?.GetCustomAttribute<WebpageOutputCacheableAttribute>();
                if (attribute == null)
                    return CachingInfo.DoNotCache;

                var shouldCache = !webpage.DoNotCache && CurrentRequestData.CurrentUser == null &&
                                  !_settings.CacheDisabled(type);
                return new CachingInfo(shouldCache, GetCacheKey(webpage, queryData),
                    TimeSpan.FromSeconds(attribute.Length),
                    attribute.ExpiryType);
            }
        }

        private string GetCacheKey(Webpage webpage, object queryData = null)
        {
            var cacheKey = $"Webpage.{webpage.Id}";
            var routingData = _querySerializer.GetRoutingData(queryData);
            cacheKey = _querySerializer.AppendToUrl(cacheKey, routingData);
            return cacheKey;
        }
    }
}