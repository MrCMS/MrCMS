using System;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class GetWidgetCachingInfo : IGetWidgetCachingInfo
    {
        private readonly IGetCurrentPage _getCurrentPage;
        private readonly IGetCurrentUser _getCurrentUser;

        public GetWidgetCachingInfo(IGetCurrentPage getCurrentPage, IGetCurrentUser getCurrentUser)
        {
            _getCurrentPage = getCurrentPage;
            _getCurrentUser = getCurrentUser;
        }
        public CachingInfo Get(Widget widget)
        {
            //using (MiniProfiler.Current.Step("Get caching info"))
            {
                if (widget == null)
                {
                    return CachingInfo.DoNotCache;
                }
                var attribute = widget.GetType().GetCustomAttribute<WidgetOutputCacheableAttribute>();
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

        private string GetCacheKey(Widget widget, WidgetOutputCacheableAttribute attribute)
        {
            var cacheKey = "Widget." + widget.Id;
            if (attribute.PerPage)
            {
                var page = _getCurrentPage.GetPage();
                if (page != null)
                {
                    cacheKey += ".Page:" + page.Id;
                }
            }
            if (attribute.PerUser)
            {
                cacheKey += ".User:" + (_getCurrentUser.Get()?.Guid ?? Guid.Empty); // TODO: user guid for anon
            }
            return cacheKey;
        }

    }
}