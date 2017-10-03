using System;
using System.Web.Mvc;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;
using MrCMS.Website.Caching;

namespace MrCMS.Services.Caching
{
    public class HtmlCacheService : IHtmlCacheService
    {
        private readonly ICacheManager _cacheManager;

        public HtmlCacheService(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager;
        }

        public ActionResult GetContent(Controller controller, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func)
        {
            return _cacheManager.Get(cachingInfo.CacheKey, () => new ContentResult
            {
                Content = func(controller.GetHtmlHelper()).ToString()
            }, cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero, cachingInfo.ExpiryType);
        }

        public MvcHtmlString GetString(Controller controller, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func)
        {
            return _cacheManager.Get(cachingInfo.CacheKey, () => func(controller.GetHtmlHelper()),
                cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero, cachingInfo.ExpiryType);
        }

        public ActionResult GetContent(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func)
        {
            return _cacheManager.Get(cachingInfo.CacheKey, () => new ContentResult
            {
                Content = func(helper).ToString()
            }, cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero, cachingInfo.ExpiryType);
        }

        public MvcHtmlString GetString(IHtmlHelper helper, CachingInfo cachingInfo, Func<IHtmlHelper, MvcHtmlString> func)
        {
            return _cacheManager.Get(cachingInfo.CacheKey, () => func(helper),
                cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero, cachingInfo.ExpiryType);
        }
    }
}