using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Areas.Admin.Hubs;
using MrCMS.Web.Areas.Admin.Models.Notifications;

namespace MrCMS.Web.Areas.Admin.Services.Events
{
    public class SendPersistentNotificationToHub : IOnPersistentNotificationPublished
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SendPersistentNotificationToHub(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Execute(OnPersistentNotificationPublishedEventArgs args)
        {
            var notification = args.Notification;
            var model = new NotificationModel { Message = notification.Message, DateValue = notification.CreatedOn };
            switch (notification.NotificationType)
            {
                case NotificationType.AdminOnly:
                    await _hubContext.Clients.Group(NotificationHub.AdminGroup)
                        .SendAsync("sendPersistentNotification", model);
                    break;
                case NotificationType.UserOnly:
                    await _hubContext.Clients.Group(NotificationHub.UsersGroup)
                        .SendAsync("sendPersistentNotification", model);
                    break;
                case NotificationType.All:
                    await _hubContext.Clients.All
                        .SendAsync("sendPersistentNotification", model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            };
        }
    }
}