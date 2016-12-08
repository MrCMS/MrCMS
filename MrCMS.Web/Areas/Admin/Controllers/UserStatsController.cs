using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Models;
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

        [DashboardAreaAction(DashboardArea = DashboardArea.RightColumn, Order = 101)]
        public PartialViewResult Summary()
        {
            return PartialView(_userStatsService.GetSummary());
        }
    }
}