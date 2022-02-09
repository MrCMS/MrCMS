using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
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

        public async Task<JsonResult> Get()
        {
            return Json(await _service.GetNotifications());
        }

        public async Task<JsonResult> GetCount()
        {
            return Json(await _service.GetNotificationCount());
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