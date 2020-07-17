using System;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Admin.Hubs;
using MrCMS.Web.Admin.Models.Notifications;

namespace MrCMS.Web.Admin.Services.Events
{
    public class SendPersistentNotificationToHub : IOnPersistentNotificationPublished
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SendPersistentNotificationToHub(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public void Execute(OnPersistentNotificationPublishedEventArgs args)
        {
            var notification = args.Notification;
            var model = new NotificationModel { Message = notification.Message, DateValue = notification.CreatedOn };
            switch (notification.NotificationType)
            {
                case NotificationType.AdminOnly:
                    _hubContext.Clients.Group(NotificationHub.AdminGroup)
                        .SendAsync("sendPersistentNotification", model);
                    break;
                case NotificationType.UserOnly:
                    _hubContext.Clients.Group(NotificationHub.UsersGroup)
                        .SendAsync("sendPersistentNotification", model);
                    break;
                case NotificationType.All:
                    _hubContext.Clients.All
                        .SendAsync("sendPersistentNotification", model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
    }
}