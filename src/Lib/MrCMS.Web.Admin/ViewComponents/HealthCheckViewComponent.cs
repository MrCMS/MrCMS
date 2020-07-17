using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Services.Dashboard;
using MrCMS.Web.Admin.Infrastructure.Dashboard;

namespace MrCMS.Web.Admin.ViewComponents
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