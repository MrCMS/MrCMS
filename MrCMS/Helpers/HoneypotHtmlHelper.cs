using System.Web.Mvc;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Helpers
{
    public static class HoneypotHtmlHelper
    {
        public static MvcHtmlString Honeypot(this HtmlHelper html)
        {
            var siteSettings = MrCMSApplication.Get<SiteSettings>();

            return siteSettings.HasHoneyPot
                       ? MvcHtmlString.Create(siteSettings.GetHoneypot().ToString())
                       : MvcHtmlString.Empty;
        }
    }
}