using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class SiteCloneOptionsController : MrCMSAdminController
    {
        private readonly ISiteCloneOptionsAdminService _siteCloneOptionsAdminService;

        public SiteCloneOptionsController(ISiteCloneOptionsAdminService siteCloneOptionsAdminService)
        {
            _siteCloneOptionsAdminService = siteCloneOptionsAdminService;
        }

        public PartialViewResult Get()
        {
            ViewData["other-sites"] = _siteCloneOptionsAdminService.GetOtherSiteOptions();

            return PartialView(_siteCloneOptionsAdminService.GetClonePartOptions());
        }
    }
}