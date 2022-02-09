using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Installation.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Filters
{
    public class HoneypotFilter : IActionFilter
    {
        // private readonly SiteSettings _siteSettings;
        //
        // public HoneypotFilter(SiteSettings siteSettings)
        // {
        //     _siteSettings = siteSettings;
        // }
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var serviceProvider = filterContext.HttpContext.RequestServices;
            if (!serviceProvider.GetRequiredService<IDatabaseCreationService>().IsDatabaseInstalled())
                return;

            var siteSettings = serviceProvider.GetRequiredService<SiteSettings>();
            if (filterContext.HttpContext.Request.HasFormContentType &&
                !string.IsNullOrWhiteSpace(filterContext.HttpContext.Request.Form[siteSettings.HoneypotFieldName]))
            {
                filterContext.Result = new EmptyResult();
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}