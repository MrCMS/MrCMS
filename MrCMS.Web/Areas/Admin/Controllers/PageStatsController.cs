using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class PageStatsController : MrCMSAdminController
    {
        private readonly IAdminPageStatsService _pageStatsService;

        public PageStatsController(IAdminPageStatsService pageStatsService)
        {
            _pageStatsService = pageStatsService;
        }

        [DashboardAreaAction(DashboardArea = DashboardArea.LeftColumn, Order = 100)]
        [OutputCache(Duration = 3600, VaryByParam = "none")]
        public PartialViewResult Summary()
        {
            return PartialView(_pageStatsService.GetSummary());
        }
    }
}