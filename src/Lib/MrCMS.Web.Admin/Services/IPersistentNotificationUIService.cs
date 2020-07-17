using System.Collections.Generic;
using MrCMS.Web.Admin.Models.Notifications;

namespace MrCMS.Web.Admin.Services
{
    public interface IPersistentNotificationUIService
    {
        IList<NotificationModel> GetNotifications();
        int GetNotificationCount();
        void MarkAllAsRead();
    }
}