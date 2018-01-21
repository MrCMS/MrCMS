using System.Web.Mvc;
using MrCMS.Entities.Notifications;
using MrCMS.Web.Areas.Admin.ACL;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NotificationController : MrCMSAdminController
    {
        private readonly INotificationAdminService _service;

        public NotificationController(INotificationAdminService service)
        {
            _service = service;
        }

        public ViewResult Index(NotificationSearchQuery searchQuery)
        {
            ViewData["results"] = _service.Search(searchQuery);
            ViewData["notification-type-options"] = _service.GetNotificationTypeOptions(true);
            return View(searchQuery);
        }

        [HttpGet]
        [MrCMSACLRule(typeof(NotificationACL), NotificationACL.Delete)]
        public ViewResult Delete(Notification notification)
        {
            return View(notification);
        }

        [HttpPost]
        [ActionName("Delete")]
        [MrCMSACLRule(typeof(NotificationACL), NotificationACL.Delete)]
        public RedirectToRouteResult Delete_POST(Notification notification)
        {
            _service.Delete(notification);
            return RedirectToAction("Index");
        }
    }
}