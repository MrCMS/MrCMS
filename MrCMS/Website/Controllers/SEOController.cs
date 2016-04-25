using System.Text;
using System.Web.Mvc;
using MrCMS.Services.Sitemaps;
using MrCMS.Settings;
using MrCMS.Website.Filters;

namespace MrCMS.Website.Controllers
{
    public class SEOController : MrCMSUIController
    {
        private readonly SEOSettings _seoSettings;
        private readonly ISitemapService _sitemapService;

        public SEOController(ISitemapService sitemapService, SEOSettings seoSettings)
        {
            _sitemapService = sitemapService;
            _seoSettings = seoSettings;
        }

        [EnsureHomePageIsSet]
        public ActionResult Sitemap()
        {
            string content = _sitemapService.GetSitemap();
            return Content(content, "application/xml", Encoding.UTF8);
        }

        public ActionResult Robots()
        {
            if (CurrentRequestData.CurrentContext.Request.Url != null && CurrentRequestData.CurrentContext.Request.Url.Host == CurrentRequestData.CurrentSite.StagingUrl)
            {
                return Content(_seoSettings.RobotsTextStaging, "text/plain", Encoding.UTF8);
            }
            return Content(_seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }
    }
}