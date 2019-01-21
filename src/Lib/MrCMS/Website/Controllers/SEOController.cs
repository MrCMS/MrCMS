using System.Security.Policy;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Settings;

namespace MrCMS.Website.Controllers
{
    public class SEOController : MrCMSUIController
    {
        private readonly SEOSettings _seoSettings;

        public SEOController(SEOSettings seoSettings)
        {
            _seoSettings = seoSettings;
        }

        public ActionResult Robots()
        {
            if (Request.Host.Host == /*CurrentRequestData.CurrentSite.StagingUrl*/ "staging") // TODO: fix staging check
                return Content(_seoSettings.RobotsTextStaging, "text/plain", Encoding.UTF8);
            return Content(_seoSettings.RobotsText, "text/plain", Encoding.UTF8);
        }
    }
}