using System;
using System.Web.Mvc;
using MrCMS.HealthChecks;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services.Dashboard;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
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
        public JsonResult Process([IoCModelBinder(typeof(HealthCheckProcessorModelBinder))] IHealthCheck healthCheck)
        {
            return Json(healthCheck == null ? new HealthCheckResult() : healthCheck.PerformCheck(), JsonRequestBehavior.AllowGet);
        }
    }
}