using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.ACL;
using MrCMS.Web.Admin.Infrastructure.BaseControllers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;
using MrCMS.Website;

namespace MrCMS.Web.Admin.Controllers
{
    public class NotificationController : MrCMSAdminController
    {
        private readonly INotificationAdminService _service;

        public NotificationController(INotificationAdminService service)
        {
            _service = service;
        }

        public async Task<ViewResult> Index(NotificationSearchQuery searchQuery)
        {
            ViewData["results"] = await _service.Search(searchQuery);
            ViewData["notification-type-options"] = _service.GetNotificationTypeOptions(true);
            return View(searchQuery);
        }

        [HttpGet]
        public ViewResult Push()
        {
            ViewData["publish-type-options"] = _service.GetPublishTypeOptions();
            ViewData["notification-type-options"] = _service.GetNotificationTypeOptions();
            return View(new NotificationModel());
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Push(NotificationModel model)
        {
            await _service.PushNotification(model);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Acl(typeof(NotificationACL), NotificationACL.Delete)]
        public async Task<ViewResult> Delete(int id)
        {
            var notification = await _service.GetNotification(id);
            return View(notification);
        }

        [HttpPost]
        [ActionName("Delete")]
        [Acl(typeof(NotificationACL), NotificationACL.Delete)]
        public async Task<RedirectToActionResult> Delete_POST(int id)
        {
            var notification = await _service.GetNotification(id);
            await _service.Delete(notification);
            return RedirectToAction("Index");
        }
    }
}