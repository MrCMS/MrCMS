using System.Web.Mvc;
using MrCMS.Entities.People;
using MrCMS.Entities.UserProfile;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class NotificationSettingsController : MrCMSAdminController
    {
        private readonly INotificationSettingsAdminService _adminService;

        public NotificationSettingsController(INotificationSettingsAdminService adminService)
        {
            _adminService = adminService;
        }

        [ChildActionOnly]
        public PartialViewResult Show(NotificationSettings settings, User user)
        {
            if (settings == null)
                settings = _adminService.InitializeSettings(user);
            return PartialView(settings);
        }

        [HttpPost]
        public RedirectToRouteResult Update(NotificationSettings settings)
        {
            _adminService.Update(settings);
            return RedirectToAction("Edit", "User", new { id = settings.User.Id });
        }
    }
}