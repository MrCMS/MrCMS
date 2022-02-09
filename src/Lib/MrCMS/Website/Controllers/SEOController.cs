using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class SEOController : MrCMSUIController
    {
        private readonly SEOSettings _seoSettings;
        private readonly ICurrentSiteLocator _siteLocator;

        public SEOController(SEOSettings seoSettings, ICurrentSiteLocator siteLocator)
        {
            _seoSettings = seoSettings;
            _siteLocator = siteLocator;
        }

        [Route("robots.txt"), HttpGet]
        public ActionResult Robots()
        {
            var site = _siteLocator.GetCurrentSite();
            return Content(
                Request.Host.Host.Equals(site.StagingUrl, StringComparison.InvariantCultureIgnoreCase)
                    ? _seoSettings.RobotsTextStaging
                    : _seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }
    }
}