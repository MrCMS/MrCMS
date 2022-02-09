using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class SiteCloneOptionsController : MrCMSAdminController
    {
        private readonly ISiteCloneOptionsAdminService _siteCloneOptionsAdminService;

        public SiteCloneOptionsController(ISiteCloneOptionsAdminService siteCloneOptionsAdminService)
        {
            _siteCloneOptionsAdminService = siteCloneOptionsAdminService;
        }

        public async Task<PartialViewResult> Get()
        {
            ViewData["other-sites"] = await _siteCloneOptionsAdminService.GetOtherSiteOptions();

            return PartialView(_siteCloneOptionsAdminService.GetClonePartOptions());
        }
    }
}