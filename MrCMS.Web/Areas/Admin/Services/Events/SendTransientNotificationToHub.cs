using System;
using Microsoft.AspNet.SignalR;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Areas.Admin.Hubs;
using MrCMS.Web.Areas.Admin.Models.Notifications;
using MrCMS.Website;

namespace MrCMS.Web.Areas.Admin.Services.Events
{
    public class SendTransientNotificationToHub : IOnTransientNotificationPublished
    {
        public void Execute(OnTransientNotificationPublishedEventArgs args)
        {
            IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            var notification = args.Notification;
            var model = new NotificationModel {Message = notification.Message, DateValue = CurrentRequestData.Now};
            switch (notification.NotificationType)
            {
                case NotificationType.AdminOnly:
                    hubContext.Clients.Group(NotificationHub.AdminGroup).sendTransientNotification(model);
                    break;
                case NotificationType.UserOnly:
                    hubContext.Clients.Group(NotificationHub.UsersGroup).sendTransientNotification(model);
                    break;
                case NotificationType.All:
                    hubContext.Clients.All.sendTransientNotification(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}