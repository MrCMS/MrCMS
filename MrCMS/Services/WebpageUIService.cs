using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services.Caching;
using MrCMS.Website;
using MrCMS.Website.Filters;

namespace MrCMS.Services
{
    public class WebpageUIService : IWebpageUIService
    {
        private readonly IHtmlCacheService _htmlCacheService;
        private readonly IGetWebpageCachingInfo _getWebpageCachingInfo;

        public WebpageUIService(IHtmlCacheService htmlCacheService, IGetWebpageCachingInfo getWebpageCachingInfo)
        {
            _htmlCacheService = htmlCacheService;
            _getWebpageCachingInfo = getWebpageCachingInfo;
        }

        public ActionResult GetContent(Controller controller, Webpage webpage, Func<IHtmlHelper, MvcHtmlString> func,
            object queryData = null)
        {
            // This will only exist if the previous request explicitly stated to cache-bust (e.g. form submission)
            var doNotCache = controller?.TempData?[DoNotCacheAttribute.TempDataKey] is bool b && b;

            // if it's explicit, we always pass a non-caching CachingInfo instance to the service, so the page is rendered as per usual
            var cachingInfo = doNotCache ? CachingInfo.DoNotCache : _getWebpageCachingInfo.Get(webpage, queryData);

            return _htmlCacheService.GetContent(controller, cachingInfo, func);
        }
    }
}