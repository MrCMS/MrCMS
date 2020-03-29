using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.Dashboard;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Areas.Admin.ViewComponents
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