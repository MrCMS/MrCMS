using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Services.Dashboard;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
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