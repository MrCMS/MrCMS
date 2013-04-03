using System.Text;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Controllers
{
    public class SEOController : MrCMSUIController
    {
        private readonly INavigationService _navigationService;
        private readonly SEOSettings _seoSettings;

        public SEOController(INavigationService navigationService, SEOSettings seoSettings)
        {
            _navigationService = navigationService;
            _seoSettings = seoSettings;
        }

        public ActionResult Sitemap()
        {
            var content = _navigationService.GetSiteMap(Url);
            return Content(content, "application/xml", Encoding.UTF8);
        }

        public ActionResult Robots()
        {
            return Content(_seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }
    }
}