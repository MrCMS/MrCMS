using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.Dashboard;
using MrCMS.Web.Areas.Admin.Services.Dashboard;

namespace MrCMS.Web.Areas.Admin.ViewComponents
{
    [DashboardArea(Area = DashboardArea.RightColumn, Order = 100)]
    public class HealthCheckViewComponent : ViewComponent
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckViewComponent(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_healthCheckService.GetHealthChecks());
        }
    }
}