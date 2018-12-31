using System.Collections.Generic;
using MrCMS.Web.Apps.Admin.Models.Notifications;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IPersistentNotificationUIService
    {
        IList<NotificationModel> GetNotifications();
        int GetNotificationCount();
        void MarkAllAsRead();
    }
}