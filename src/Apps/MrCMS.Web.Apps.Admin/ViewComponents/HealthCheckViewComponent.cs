using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Infrastructure.Dashboard;
using MrCMS.Web.Apps.Admin.Services.Dashboard;

namespace MrCMS.Web.Apps.Admin.ViewComponents
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