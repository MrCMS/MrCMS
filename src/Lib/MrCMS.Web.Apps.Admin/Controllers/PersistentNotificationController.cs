using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
            return Json(_service.GetNotifications());
        }

        public JsonResult GetCount()
        {
            return Json(_service.GetNotificationCount());
        }

        public PartialViewResult Navbar()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<JsonResult> MarkAllAsRead()
        {
            await _service.MarkAllAsRead();
            return Json(true);
        }
    }
}