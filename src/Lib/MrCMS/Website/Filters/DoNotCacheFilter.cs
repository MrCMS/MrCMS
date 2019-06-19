using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website.Filters
{
    /// <summary>
    /// This filter tells Mr CMS not to cache the next response if the page is in memory cache. We check to see if the request method is POST, and if so we insert a TempData key which we use in WebpageCachingFilter to uncache the page.
    /// </summary>
    public class DoNotCacheFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // if something has gone wrong, ignore this
            if (filterContext.Exception != null)
                return;
            if (!(filterContext.Controller is Controller controller))
                return;
            // if it exists at this point, clear it, as it's left over from the previous request
            if (controller.TempData.ContainsKey(DoNotCacheKey.TempDataKey))
                controller.TempData.Remove(DoNotCacheKey.TempDataKey);

            if (controller.Request.Method == HttpMethod.Post.Method)
            {
                controller.TempData[DoNotCacheKey.TempDataKey] = true;
                controller.TempData.Keep();
            }
        }
    }
}