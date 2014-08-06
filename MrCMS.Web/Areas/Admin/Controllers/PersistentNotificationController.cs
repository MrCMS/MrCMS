using System;
using System.Web.Mvc;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class PersistentNotificationController : MrCMSAdminController
    {
        private readonly IPersistentNotificationUIService _service;

        public PersistentNotificationController(IPersistentNotificationUIService service)
        {
            _service = service;
        }

        public PartialViewResult Show()
        {
            return PartialView();
        }

        public JsonResult Get()
        {
            return Json(_service.GetNotifications(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCount()
        {
            return Json(_service.GetNotificationCount(), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult Navbar()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult MarkAllAsRead()
        {
            _service.MarkAllAsRead();
            return Json(true);
        }
    }
}