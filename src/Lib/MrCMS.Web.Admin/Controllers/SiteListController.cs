using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class SiteListController : MrCMSAdminController
    {
        private readonly IAdminSiteListService _siteListService;

        public SiteListController(IAdminSiteListService siteListService)
        {
            _siteListService = siteListService;
        }

        public ActionResult Get()
        {
            return PartialView(_siteListService.GetSites());
        }
    }
}