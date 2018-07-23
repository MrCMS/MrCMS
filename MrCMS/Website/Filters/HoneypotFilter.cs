using Microsoft.AspNetCore.Mvc.Filters;

namespace MrCMS.Website.Filters
{
    public class HoneypotFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (!CurrentRequestData.DatabaseIsInstalled)
            //    return;
            //if (!string.IsNullOrWhiteSpace(
            //    filterContext.HttpContext.Request[MrCMSApplication.Get<SiteSettings>().HoneypotFieldName]))
            //{
            //    filterContext.Result = new EmptyResult();
            //}
            // TODO: honeypot
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}