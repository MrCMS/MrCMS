using System.Text;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Web.Controllers
{
    public class SEOController : Controller
    {
        private readonly INavigationService _navigationService;
        private readonly SEOSettings _seoSettings;
        private readonly ISiteService _siteService;

        public SEOController(INavigationService navigationService, SEOSettings seoSettings, ISiteService siteService)
        {
            _navigationService = navigationService;
            _seoSettings = seoSettings;
            _siteService = siteService;
        }

        public ActionResult Sitemap()
        {
            var content = _navigationService.GetSiteMap(Url, _siteService.GetCurrentSite());
            return Content(content, "application/xml", Encoding.UTF8);
        }

        public ActionResult Robots()
        {
            return Content(_seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }

        public ActionResult RenderAnalytics()
        {
            if (string.IsNullOrWhiteSpace(_seoSettings.GoogleAnalytics))
                return new EmptyResult();

            return Content(_seoSettings.GoogleAnalytics);
        }
    }
}