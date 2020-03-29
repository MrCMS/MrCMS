using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Areas.Admin.Models.Notifications;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IPersistentNotificationUIService
    {
        IList<NotificationModel> GetNotifications();
        int GetNotificationCount();
        Task MarkAllAsRead();
    }
}