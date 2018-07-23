using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website.Filters
{
    public class ReturnUrlHandlerAttribute : ActionFilterAttribute
    {
        public const string ReturnUrlKey = "return-url";
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is RedirectToRouteResult || filterContext.Result is RedirectResult)
            {
                var returnUrl = filterContext.HttpContext.Request.Form[ReturnUrlKey].FirstOrDefault()
                                ?? filterContext.HttpContext.Request.Query[ReturnUrlKey].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    filterContext.Result = new RedirectResult(returnUrl);
                }
            }
        }
    }
}