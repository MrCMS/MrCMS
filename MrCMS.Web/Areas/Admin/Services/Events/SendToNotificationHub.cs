using System;
using Microsoft.AspNet.SignalR;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Areas.Admin.Hubs;

namespace MrCMS.Web.Areas.Admin.Services.Events
{
    public class SendToNotificationHub : IOnTransientNotificationPublished
    {
        public void Execute(OnTransientNotificationPublishedEventArgs args)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var notification = new NotificationModel { Message = args.Notification.Message };
            switch (args.Notification.NotificationType)
            {
                case NotificationType.AdminOnly:
                    hubContext.Clients.Group(NotificationHub.AdminGroup).sendTransientNotification(notification);
                    break;
                case NotificationType.UserOnly:
                    hubContext.Clients.Group(NotificationHub.UsersGroup).sendTransientNotification(notification);
                    break;
                case NotificationType.All:
                    hubContext.Clients.All.sendTransientNotification(notification);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class NotificationModel
    {
        public string Message { get; set; }
    }
}