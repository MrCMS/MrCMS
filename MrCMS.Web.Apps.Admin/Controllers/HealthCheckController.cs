using Microsoft.AspNetCore.Mvc;
using MrCMS.HealthChecks;
using MrCMS.Web.Apps.Admin.Services.Dashboard;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class HealthCheckController : MrCMSAdminController
    {
        private readonly IHealthCheckService _service;

        public HealthCheckController(IHealthCheckService service)
        {
            _service = service;
        }

        [HttpGet]
        public JsonResult Process(string typeName)
        {
            return Json(_service.CheckType(typeName));
        }
    }
}