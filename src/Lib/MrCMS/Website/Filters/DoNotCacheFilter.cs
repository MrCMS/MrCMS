using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
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
            // if something has gone wrong, ignore this
            if (filterContext.Exception != null)
                return;
            if (!(filterContext.Controller is Controller controller))
                return;
            // if it exists at this point, clear it, as it's left over from the previous request
            if (controller.TempData.ContainsKey(DoNotCacheAttribute.TempDataKey))
                controller.TempData.Remove(DoNotCacheAttribute.TempDataKey);

            // re-add it for the current action
            var attributes =
                (filterContext.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.GetCustomAttributes<DoNotCacheAttribute>(true) ?? Enumerable.Empty<DoNotCacheAttribute>();
            if (attributes.Any())
            {
                controller.TempData[DoNotCacheAttribute.TempDataKey] = true;
                controller.TempData.Keep();
            }
        }
    }
}