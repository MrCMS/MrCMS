using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Settings;

namespace MrCMS.Website.Filters
{
    public class HoneypotFilter : IActionFilter
    {
        private readonly SiteSettings _siteSettings;

        public HoneypotFilter(SiteSettings siteSettings)
        {
            _siteSettings = siteSettings;
        }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.HasFormContentType &&
                !string.IsNullOrWhiteSpace( filterContext.HttpContext.Request.Form[_siteSettings.HoneypotFieldName]))
            {
                filterContext.Result = new EmptyResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}