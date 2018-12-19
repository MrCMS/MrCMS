using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Infrastructure.Dashboard;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.ViewComponents
{
    [DashboardArea(Area = DashboardArea.LeftColumn, Order = 100)]
    public class PageStatsViewComponent : ViewComponent
    {
        private readonly IAdminPageStatsService _userStatsService;

        public PageStatsViewComponent(IAdminPageStatsService userStatsService)
        {
            _userStatsService = userStatsService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_userStatsService.GetSummary());
        }
    }
}