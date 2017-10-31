using System.Linq;
using System.Web.Mvc;

namespace MrCMS.Website.Filters
{
    public class DoNotCacheFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // if something has gone wrong, or it's a child action, ignore this
            if (filterContext.Exception != null || filterContext.IsChildAction)
                return;
            // if it exists at this point, clear it, as it's left over from the previous request
            if (filterContext.Controller.TempData.ContainsKey(DoNotCacheAttribute.TempDataKey))
                filterContext.Controller.TempData.Remove(DoNotCacheAttribute.TempDataKey);

            // re-add it for the current action
            var attributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof(DoNotCacheAttribute), true);
            if (attributes.Any())
            {
                filterContext.Controller.TempData[DoNotCacheAttribute.TempDataKey] = true;
                filterContext.Controller.TempData.Keep();
            }
        }

    }
}