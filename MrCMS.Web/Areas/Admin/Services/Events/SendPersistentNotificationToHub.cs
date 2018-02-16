using System;
using Microsoft.AspNet.SignalR;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Areas.Admin.Hubs;
using MrCMS.Web.Areas.Admin.Models.Notifications;

namespace MrCMS.Web.Areas.Admin.Services.Events
{
    public class SendPersistentNotificationToHub : IOnPersistentNotificationPublished
    {
        public void Execute(OnPersistentNotificationPublishedEventArgs args)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var notification = args.Notification;
            var model = new NotificationModel {Message = notification.Message, DateValue = notification.CreatedOn};
            switch (notification.NotificationType)
            {
                case NotificationType.AdminOnly:
                    hubContext.Clients.Group(NotificationHub.AdminGroup).sendPersistentNotification(model);
                    break;
                case NotificationType.UserOnly:
                    hubContext.Clients.Group(NotificationHub.UsersGroup).sendPersistentNotification(model);
                    break;
                case NotificationType.All:
                    hubContext.Clients.All.sendPersistentNotification(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
    }
}