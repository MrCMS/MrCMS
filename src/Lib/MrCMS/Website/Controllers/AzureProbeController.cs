using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Website.Controllers
{
    public class AzureProbeController : MrCMSUIController
    {
        private readonly AzureProbeSettings _azureProbeSettings;
        private readonly IStatelessSession _session;

        public AzureProbeController(AzureProbeSettings azureProbeSettings, IStatelessSession session)
        {
            _azureProbeSettings = azureProbeSettings;
            _session = session;
        }

        //[Route("KeepAlive")]
        //[HttpGet()]
        public ActionResult KeepAlive()
        {
            var item = HttpContext.Request.Query[_azureProbeSettings.Key].ToString();
            if (string.IsNullOrWhiteSpace(item) || item != _azureProbeSettings.Password)
                return new StatusCodeResult(403);

            return new StatusCodeResult(!_session.QueryOver<Site>().Any() ? 500 : 200);
        }
    }
}