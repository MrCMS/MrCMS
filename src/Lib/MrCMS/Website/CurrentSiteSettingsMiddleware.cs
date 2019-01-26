using Microsoft.AspNetCore.Http;
using MrCMS.Settings;
using System.Threading.Tasks;

namespace MrCMS.Website
{
    public class CurrentSiteSettingsMiddleware : IMiddleware
    {
        public const string Key = "Current.SiteSettings";
        private readonly IConfigurationProvider _configurationProvider;

        public CurrentSiteSettingsMiddleware(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Items[Key] = _configurationProvider.GetSiteSettings<SiteSettings>();
            await next(context);
        }
    }
}