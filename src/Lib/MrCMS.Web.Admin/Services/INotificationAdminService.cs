using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Notifications;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface INotificationAdminService
    {
        Task<IPagedList<Notification>> Search(NotificationSearchQuery searchQuery);
        Task PushNotification(NotificationModel model);
        List<SelectListItem> GetPublishTypeOptions();
        List<SelectListItem> GetNotificationTypeOptions(bool includeAnyOption = false);
        Task Delete(Notification notification);
        Task<Notification> GetNotification(int id);
    }
}