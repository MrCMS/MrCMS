using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.Dashboard;
using MrCMS.Web.Areas.Admin.Services;

namespace MrCMS.Web.Areas.Admin.ViewComponents
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