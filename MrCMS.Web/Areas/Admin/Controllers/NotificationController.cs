using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MrCMS.Services.Notifications;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
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
            return View(searchQuery);
        }

        [HttpGet]
        public ViewResult Push()
        {
            ViewData["publish-type-options"] = _service.GetPublishTypeOptions();
            return View(new PushNotificationModel());
        }
        [HttpPost]
        public RedirectToRouteResult Push(PushNotificationModel model)
        {
            _service.PushNotification(model);
            return RedirectToAction("Index");
        }
    }

    public class PushNotificationModel
    {
        [Required]
        public string Message { get; set; }
        [DisplayName("Publish Type")]
        public PublishType PublishType { get; set; }
    }
}