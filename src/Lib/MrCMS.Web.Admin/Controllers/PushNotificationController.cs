using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Admin.Controllers
{
    public class PushNotificationController : MrCMSAdminController
    {
        private readonly IPushNotificationAdminService _service;

        public PushNotificationController(IPushNotificationAdminService service)
        {
            _service = service;
        }

        public ViewResult Index()
        {
            return View();
        }

        public JsonResult Push(PushNotificationModel model)
        {
            return Json(_service.PushToAll(model));
        }
    }
}