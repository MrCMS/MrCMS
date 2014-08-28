using System.Text;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class SEOController : MrCMSUIController
    {
        private readonly ISiteMapService _siteMapService;
        private readonly SEOSettings _seoSettings;

        public SEOController(ISiteMapService siteMapService, SEOSettings seoSettings)
        {
            _siteMapService = siteMapService;
            _seoSettings = seoSettings;
        }

        public ActionResult Sitemap()
        {
            var content = _siteMapService.GetSiteMap();
            return Content(content, "application/xml", Encoding.UTF8);
        }

        public ActionResult Robots()
        {
            return Content(_seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }
    }
}