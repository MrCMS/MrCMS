using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Notifications;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface INotificationAdminService
    {
        IPagedList<Notification> Search(NotificationSearchQuery searchQuery);
        void PushNotification(NotificationModel model);
        List<SelectListItem> GetPublishTypeOptions();
        List<SelectListItem> GetNotificationTypeOptions(bool includeAnyOption = false);
        Task Delete(Notification notification);
    }
}