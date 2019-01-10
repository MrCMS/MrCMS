using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Settings;

namespace MrCMS.Helpers
{
    public static class HoneypotHtmlHelper
    {
        public static IHtmlContent Honeypot(this IHtmlHelper html)
        {
            var siteSettings = html.GetRequiredService<SiteSettings>();

            return siteSettings.HasHoneyPot
                ? (IHtmlContent) siteSettings.GetHoneypot()
                : HtmlString.Empty;
        }
    }
}