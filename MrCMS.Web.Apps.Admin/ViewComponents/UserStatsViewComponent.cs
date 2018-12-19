using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Infrastructure.Dashboard;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.ViewComponents
{
    [DashboardArea(Area = DashboardArea.RightColumn, Order = 101)]
    public class UserStatsViewComponent : ViewComponent
    {
        private readonly IAdminUserStatsService _userStatsService;

        public UserStatsViewComponent(IAdminUserStatsService userStatsService)
        {
            _userStatsService = userStatsService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_userStatsService.GetSummary());
        }
    }
}