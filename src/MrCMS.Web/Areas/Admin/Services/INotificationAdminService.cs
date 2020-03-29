using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Notifications;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface INotificationAdminService
    {
        IPagedList<Notification> Search(NotificationSearchQuery searchQuery);
        Task PushNotification(NotificationModel model);
        List<SelectListItem> GetPublishTypeOptions();
        List<SelectListItem> GetNotificationTypeOptions(bool includeAnyOption = false);
        Task Delete(Notification notification);
    }
}