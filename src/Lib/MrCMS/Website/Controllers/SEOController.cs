using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class SEOController : MrCMSUIController
    {
        private readonly IGetCurrentSite _getCurrentSite;
        private readonly IConfigurationProvider _configurationProvider;

        public SEOController(IGetCurrentSite getCurrentSite, IConfigurationProvider configurationProvider)
        {
            _getCurrentSite = getCurrentSite;
            _configurationProvider = configurationProvider;
        }

        public async Task<ActionResult> Robots()
        {
            var site = await _getCurrentSite.GetSite();
            var seoSettings = await _configurationProvider.GetSiteSettings<SEOSettings>();
            return Content(
                Request.Host.Host.Equals(site.StagingUrl, StringComparison.InvariantCultureIgnoreCase)
                    ? seoSettings.RobotsTextStaging
                    : seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }
    }
}