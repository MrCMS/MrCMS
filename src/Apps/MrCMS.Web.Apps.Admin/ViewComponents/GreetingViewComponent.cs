using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Infrastructure.Dashboard;

namespace MrCMS.Web.Apps.Admin.ViewComponents
{
    [DashboardArea(Area = DashboardArea.Top, Order = -1)]
    public class GreetingViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}