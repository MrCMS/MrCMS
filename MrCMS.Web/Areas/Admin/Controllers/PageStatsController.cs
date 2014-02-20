using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class UserStatsController : MrCMSAdminController
    {
        private readonly IAdminUserStatsService _userStatsService;

        public UserStatsController(IAdminUserStatsService userStatsService)
        {
            _userStatsService = userStatsService;
        }

        [DashboardAreaAction(DashboardArea = DashboardArea.RightColumn)]
        public PartialViewResult Summary()
        {
            return PartialView(_userStatsService.GetSummary());
        }
    }

    public class PageStatsController : MrCMSAdminController
    {
        private readonly IAdminPageStatsService _pageStatsService;

        public PageStatsController(IAdminPageStatsService pageStatsService)
        {
            _pageStatsService = pageStatsService;
        }

        [DashboardAreaAction(DashboardArea = DashboardArea.LeftColumn)]
        public PartialViewResult Summary()
        {
            return PartialView(_pageStatsService.GetSummary());
        }
    }
}