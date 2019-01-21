using System.Text;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class SEOController : MrCMSUIController
    {
        private readonly SEOSettings _seoSettings;
        private readonly Site _site;

        public SEOController(SEOSettings seoSettings, Site site)
        {
            _seoSettings = seoSettings;
            _site = site;
        }

        public ActionResult Robots()
        {
            return Content(
                Request.Host.Host == _site.StagingUrl ? _seoSettings.RobotsTextStaging : _seoSettings.RobotsText,
                "text/plain", Encoding.UTF8);
        }
    }
}