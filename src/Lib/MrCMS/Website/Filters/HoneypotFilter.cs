using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MrCMS.Settings;

namespace MrCMS.Website.Filters
{
    public class HoneypotFilter : IAsyncActionFilter
    {
        private readonly IConfigurationProvider _configurationProvider;

        public HoneypotFilter(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var siteSettings = await _configurationProvider.GetSiteSettings<SiteSettings>();
            if (context.HttpContext.Request.HasFormContentType &&
                !string.IsNullOrWhiteSpace( context.HttpContext.Request.Form[siteSettings.HoneypotFieldName]))
            {
                context.Result = new EmptyResult();
                return;
            }

            await next();
        }
    }
}