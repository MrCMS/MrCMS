using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.Dashboard;

namespace MrCMS.Web.Areas.Admin.ViewComponents
{
    [DashboardArea(Area = DashboardArea.Top, Order = -1)]
    public class GreetingViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}