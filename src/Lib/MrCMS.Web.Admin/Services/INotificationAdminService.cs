using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Notifications;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Controllers;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface INotificationAdminService
    {
        IPagedList<Notification> Search(NotificationSearchQuery searchQuery);
        void PushNotification(NotificationModel model);
        List<SelectListItem> GetPublishTypeOptions();
        List<SelectListItem> GetNotificationTypeOptions(bool includeAnyOption = false);
        void Delete(Notification notification);
    }
}