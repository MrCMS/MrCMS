using System;
using System.Web.Mvc;
using MrCMS.HealthChecks;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.Services.Dashboard;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;
using Ninject;

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

    public class HealthCheckProcessorModelBinder : MrCMSDefaultModelBinder
    {
        private readonly IKernel _kernel;

        public HealthCheckProcessorModelBinder(IKernel kernel, ISession session)
            : base(() => session)
        {
            _kernel = kernel;
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string typeName = GetValueFromContext(controllerContext, "typeName");
            var typeByName = TypeHelper.GetTypeByName(typeName);
            return typeName == null ? null : _kernel.Get(typeByName);
        }
    }
}