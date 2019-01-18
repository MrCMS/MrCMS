using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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