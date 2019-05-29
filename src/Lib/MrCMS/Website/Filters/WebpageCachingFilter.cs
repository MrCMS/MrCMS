using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Caching;
using MrCMS.Website.CMS;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Website.Filters
{
    public class WebpageCachingFilter : IAsyncActionFilter
    {
        private readonly ICacheManager _cacheManager;
        private readonly IGetWebpageCachingInfo _getWebpageCachingInfo;
        private readonly IGetControllerFromFilterContext _getControllerFromFilterContext;

        public WebpageCachingFilter(ICacheManager cacheManager, IGetWebpageCachingInfo getWebpageCachingInfo, IGetControllerFromFilterContext getControllerFromFilterContext)
        {
            _cacheManager = cacheManager;
            _getWebpageCachingInfo = getWebpageCachingInfo;
            _getControllerFromFilterContext = getControllerFromFilterContext;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var isCMSRequest = context.IsCMSRequest();
            if (!isCMSRequest)
            {
                await next();
                return;
            }
            var webpage = context.ActionArguments.Values.OfType<Webpage>().FirstOrDefault();
            if (webpage == null)
            {
                await next();
                return;
            }

            var controller = _getControllerFromFilterContext.GetController(context);
            // This will only exist if the previous request explicitly stated to cache-bust (e.g. form submission)
            var doNotCache = controller?.TempData?[DoNotCacheKey.TempDataKey] is bool b && b;
            var queryData = context.HttpContext.Request.Query;

            // if it's explicit, we always pass a non-caching CachingInfo instance to the service, so the page is rendered as per usual
            var cachingInfo = doNotCache ? CachingInfo.DoNotCache : _getWebpageCachingInfo.Get(webpage, queryData);

            if (!cachingInfo.ShouldCache)
            {
                await next();
                return;
            }

            var key = GetKey(webpage);
            var actionResult = _cacheManager.Get<IActionResult>(key);
            if (actionResult != null)
            {
                context.Result = actionResult;
                return;
            }

            var executedContext = await next();

            if (executedContext.Exception == null)
            {
                _cacheManager.Set(key, executedContext.Result, cachingInfo.TimeToCache, cachingInfo.ExpiryType);
            }
        }

        private string GetKey(Webpage webpage)
        {
            return $"ActionResult.Webpage.{webpage.Id}";
        }
    }
}