using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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