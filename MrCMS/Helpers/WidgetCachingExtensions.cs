using System;
using System.Reflection;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class WidgetCachingExtensions
    {
        public static CachingInfo GetCachingInfo(this Widget widget)
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
            return new CachingInfo(shouldCache, GetCacheKey(widget, attribute), TimeSpan.FromSeconds(widget.CacheLength),
                widget.CacheExpiryType);
        }

        private static string GetCacheKey(Widget widget, OutputCacheableAttribute attribute)
        {
            var cacheKey = "Widget." + widget.Id;
            if (attribute.PerPage)
            {
                if (CurrentRequestData.CurrentPage != null)
                {
                    cacheKey += ".Page:" + CurrentRequestData.CurrentPage.Id;
                }
            }
            if (attribute.PerUser)
            {
                cacheKey += ".User:" + CurrentRequestData.UserGuid;
            }
            return cacheKey;
        }

        public static bool IsTypeCachable(this Widget widget)
        {
            return widget.GetType().GetCustomAttribute<OutputCacheableAttribute>() != null;
        }
    }
}