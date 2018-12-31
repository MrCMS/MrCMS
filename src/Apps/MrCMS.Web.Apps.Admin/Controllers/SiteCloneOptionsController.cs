using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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