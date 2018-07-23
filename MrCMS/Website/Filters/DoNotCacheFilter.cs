using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website.Filters
{
    public class DoNotCacheFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //// if something has gone wrong, ignore this
            //if (filterContext.Exception != null )
            //    return;
            //// if it exists at this point, clear it, as it's left over from the previous request
            //if (filterContext.Controller.TempData.ContainsKey(DoNotCacheAttribute.TempDataKey))
            //    filterContext.Controller.TempData.Remove(DoNotCacheAttribute.TempDataKey);

            //// re-add it for the current action
            //var attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(DoNotCacheAttribute), true);
            //if (attributes.Any())
            //{
            //    filterContext.Controller.TempData[DoNotCacheAttribute.TempDataKey] = true;
            //    filterContext.Controller.TempData.Keep();
            //}
            // TODO: do not cache
        }

    }
}