using System;
using System.Reflection;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Website;
using StackExchange.Profiling;

namespace MrCMS.Services
{
    public class GetWidgetCachingInfo : IGetWidgetCachingInfo
    {
        public CachingInfo Get(Widget widget)
        {
            using (MiniProfiler.Current.Step("Get caching info"))
            {
                if (widget == null)
                {
                    return CachingInfo.DoNotCache;
                }
                var attribute = widget.GetType().GetCustomAttribute<OutputCacheableAttribute>();
                if (attribute == null)
                {
                    return CachingInfo.DoNotCache;
                }
                var shouldCache = widget.Cache && widget.CacheLength > 0;
                return new CachingInfo(shouldCache, GetCacheKey(widget, attribute),
                    TimeSpan.FromSeconds(widget.CacheLength),
                    widget.CacheExpiryType);
            }
        }

        private static string GetCacheKey(Widget widget, OutputCacheableAttribute attribute)
        {
            var cacheKey = "Widget." + widget.Id;
            if (attribute.PerPage)
            {
                var page = CurrentRequestData.CurrentPage;
                if (page != null)
                {
                    cacheKey += ".Page:" + page.Id;
                }
            }
            if (attribute.PerUser)
            {
                cacheKey += ".User:" + CurrentRequestData.UserGuid;
            }
            return cacheKey;
        }

    }
}