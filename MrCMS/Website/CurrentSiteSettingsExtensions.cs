using Microsoft.AspNetCore.Http;
using MrCMS.Settings;

namespace MrCMS.Website
{
    public static class CurrentSiteSettingsExtensions
    {
        public static SiteSettings GetSiteSettings(this HttpContext context)
        {
            return context?.Items[CurrentSiteSettingsMiddleware.Key] as SiteSettings;
        }
    }
}