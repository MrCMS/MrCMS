using Microsoft.AspNetCore.Mvc;
using MrCMS.HealthChecks;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services.Dashboard;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class HealthCheckController : MrCMSAdminController
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [DashboardAreaAction(DashboardArea = DashboardArea.RightColumn, Order = 100)]
        public PartialViewResult List()
        {
            return PartialView(_healthCheckService.GetHealthChecks());
        }

        [HttpGet]
        public JsonResult Process([ModelBinder(typeof(HealthCheckProcessorModelBinder))] IHealthCheck healthCheck) 
        {
            return Json(healthCheck == null ? new HealthCheckResult() : healthCheck.PerformCheck());
        }
    }
}