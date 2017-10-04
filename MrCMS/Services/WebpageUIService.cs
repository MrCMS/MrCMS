using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.Caching;
using MrCMS.Website;

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
            return _htmlCacheService.GetContent(controller, _getWebpageCachingInfo.Get(webpage, queryData), func);
        }
    }
}