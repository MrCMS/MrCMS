using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Models.Notifications;

namespace MrCMS.Web.Admin.Services
{
    public interface IPersistentNotificationUIService
    {
        Task<IList<NotificationModel>> GetNotifications();
        Task<int> GetNotificationCount();
        Task MarkAllAsRead();
    }
}