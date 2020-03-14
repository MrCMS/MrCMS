using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Admin.Hubs;
using MrCMS.Web.Apps.Admin.Models.Notifications;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Admin.Services.Events
{
    public class SendTransientNotificationToHub : IOnTransientNotificationPublished
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public SendTransientNotificationToHub(IHubContext<NotificationHub> notificationHubContext, IGetDateTimeNow getDateTimeNow)
        {
            _notificationHubContext = notificationHubContext;
            _getDateTimeNow = getDateTimeNow;
        }
        public async Task Execute(OnTransientNotificationPublishedEventArgs args)
        {
            var notification = args.Notification;
            var model = new NotificationModel { Message = notification.Message, DateValue = await _getDateTimeNow.GetLocalNow() };
            switch (notification.NotificationType)
            {
                case NotificationType.AdminOnly:
                    await _notificationHubContext.Clients.Group(NotificationHub.AdminGroup)
                        .SendCoreAsync("sendTransientNotification", new object[] { model });
                    break;
                case NotificationType.UserOnly:
                    await _notificationHubContext.Clients.Group(NotificationHub.UsersGroup)
                        .SendCoreAsync("sendTransientNotification", new object[] { model });
                    break;
                case NotificationType.All:
                    await _notificationHubContext.Clients.All
                        .SendCoreAsync("sendTransientNotification", new object[] { model });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}