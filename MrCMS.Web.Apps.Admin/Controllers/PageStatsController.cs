using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class PageStatsController : MrCMSAdminController
    {
        private readonly IAdminPageStatsService _pageStatsService;

        public PageStatsController(IAdminPageStatsService pageStatsService)
        {
            _pageStatsService = pageStatsService;
        }

        [DashboardAreaAction(DashboardArea = DashboardArea.LeftColumn, Order = 100)]
        //[OutputCache(Duration = 3600, VaryByParam = "none")] // TODO: output cache?
        public PartialViewResult Summary()
        {
            return PartialView(_pageStatsService.GetSummary());
        }
    }
}