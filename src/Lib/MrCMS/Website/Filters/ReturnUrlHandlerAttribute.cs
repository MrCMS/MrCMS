using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace MrCMS.Website.Filters
{
    public class ReturnUrlHandlerAttribute : ActionFilterAttribute
    {
        public const string ReturnUrlKey = "return-url";
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is RedirectToRouteResult || filterContext.Result is RedirectResult)
            {
                string returnUrl = null;
                if (filterContext.HttpContext.Request.HasFormContentType && filterContext.HttpContext.Request.Form[ReturnUrlKey].FirstOrDefault() != null)
                {
                    returnUrl = filterContext.HttpContext.Request.Form[ReturnUrlKey].FirstOrDefault();
                }
                if (returnUrl == null)
                {
                    returnUrl = filterContext.HttpContext.Request.Query[ReturnUrlKey].FirstOrDefault();
                }

                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    filterContext.Result = new RedirectResult(returnUrl);
                }
            }
        }
    }
}