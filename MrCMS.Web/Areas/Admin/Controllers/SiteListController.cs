using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
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