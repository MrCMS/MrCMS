using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class SiteListController : MrCMSAdminController
    {
        private readonly IAdminSiteListService _siteListService;

        public SiteListController(IAdminSiteListService siteListService)
        {
            _siteListService = siteListService;
        }

        public async Task<ActionResult> Get()
        {
            return PartialView(await _siteListService.GetSites());
        }
    }
}