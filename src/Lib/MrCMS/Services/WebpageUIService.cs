using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services.Caching;
using MrCMS.Website.Caching;
using MrCMS.Website.Filters;
using ISession = NHibernate.ISession;

namespace MrCMS.Services
{
    public class WebpageUIService : IWebpageUIService
    {
        private readonly ICacheManager _cacheManager;
        private readonly IGetWebpageCachingInfo _getWebpageCachingInfo;
        private readonly ISession _session;

        public WebpageUIService(ICacheManager cacheManager, IGetWebpageCachingInfo getWebpageCachingInfo, ISession session)
        {
            _cacheManager = cacheManager;
            _getWebpageCachingInfo = getWebpageCachingInfo;
            _session = session;
        }

        public ActionResult GetContent(Controller controller, Webpage webpage, Func<ActionResult> func,
            IQueryCollection queryData = null)
        {
            // This will only exist if the previous request explicitly stated to cache-bust (e.g. form submission)
            var doNotCache = controller?.TempData?[DoNotCacheKey.TempDataKey] is bool b && b;

            // if it's explicit, we always pass a non-caching CachingInfo instance to the service, so the page is rendered as per usual
            var cachingInfo = doNotCache ? CachingInfo.DoNotCache : _getWebpageCachingInfo.Get(webpage, queryData);

            return _cacheManager.GetOrCreate(cachingInfo.CacheKey, func,
                cachingInfo.ShouldCache ? cachingInfo.TimeToCache : TimeSpan.Zero,
                cachingInfo.ExpiryType);
        }

        public T GetPage<T>(int id) where T : Webpage
        {
            return _session.Get<T>(id);
        }
    }
}