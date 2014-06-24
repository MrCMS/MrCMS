using System.Collections.Generic;
using MrCMS.Web.Areas.Admin.Models.Notifications;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IPersistentNotificationUIService
    {
        IList<NotificationModel> GetNotifications();
        int GetNotificationCount();
        void MarkAllAsRead();
    }
}