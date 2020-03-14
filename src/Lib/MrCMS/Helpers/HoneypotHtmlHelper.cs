using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Settings;

namespace MrCMS.Helpers
{
    public static class HoneypotHtmlHelper
    {
        public static async Task<IHtmlContent> Honeypot(this IHtmlHelper html)
        {
            var configurationProvider = html.GetRequiredService<IConfigurationProvider>();

            var siteSettings = await configurationProvider.GetSiteSettings<SiteSettings>();
            return siteSettings.HasHoneyPot
                ? (IHtmlContent)siteSettings.GetHoneypot()
                : HtmlString.Empty;
        }
    }
}