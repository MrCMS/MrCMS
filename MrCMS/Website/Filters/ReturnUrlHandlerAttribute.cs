using System.Web.Mvc;

namespace MrCMS.Website.Filters
{
    public class ReturnUrlHandlerAttribute : ActionFilterAttribute
    {
        public const string ReturnUrlKey = "return-url";
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Result is RedirectToRouteResult || filterContext.Result is RedirectResult)
            {
                var returnUrl = filterContext.HttpContext.Request[ReturnUrlKey];
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    filterContext.Result = new RedirectResult(returnUrl);
                }
            }
        }
    }
}