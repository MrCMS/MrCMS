using System;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Admin.Hubs;
using MrCMS.Web.Apps.Admin.Models.Notifications;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Admin.Services.Events
{
    public class SendTransientNotificationToHub : IOnTransientNotificationPublished
    {
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IGetNowForSite _getNowForSite;

        public SendTransientNotificationToHub(IHubContext<NotificationHub> notificationHubContext, IGetNowForSite getNowForSite)
        {
            _notificationHubContext = notificationHubContext;
            _getNowForSite = getNowForSite;
        }
        public void Execute(OnTransientNotificationPublishedEventArgs args)
        {
            var notification = args.Notification;
            var model = new NotificationModel { Message = notification.Message, DateValue = _getNowForSite.Now };
            switch (notification.NotificationType)
            {
                case NotificationType.AdminOnly:
                    _notificationHubContext.Clients.Group(NotificationHub.AdminGroup)
                        .SendCoreAsync("sendTransientNotification", new object[] {model}).GetAwaiter().GetResult();
                    break;
                case NotificationType.UserOnly:
                    _notificationHubContext.Clients.Group(NotificationHub.UsersGroup)
                        .SendCoreAsync("sendTransientNotification", new object[] {model}).GetAwaiter().GetResult();
                    break;
                case NotificationType.All:
                    _notificationHubContext.Clients.All
                        .SendCoreAsync("sendTransientNotification", new object[] {model}).GetAwaiter().GetResult();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}