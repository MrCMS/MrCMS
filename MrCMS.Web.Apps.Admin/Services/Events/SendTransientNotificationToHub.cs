using System;
using MrCMS.Entities.Notifications;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Admin.Models.Notifications;

namespace MrCMS.Web.Apps.Admin.Services.Events
{
    public class SendTransientNotificationToHub : IOnTransientNotificationPublished
    {
        public void Execute(OnTransientNotificationPublishedEventArgs args)
        {
            // TODO: signalR notifications
            //IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            //var notification = args.Notification;
            //var model = new NotificationModel {Message = notification.Message, DateValue = CurrentRequestData.Now};
            //switch (notification.NotificationType)
            //{
            //    case NotificationType.AdminOnly:
            //        hubContext.Clients.Group(NotificationHub.AdminGroup).sendTransientNotification(model);
            //        break;
            //    case NotificationType.UserOnly:
            //        hubContext.Clients.Group(NotificationHub.UsersGroup).sendTransientNotification(model);
            //        break;
            //    case NotificationType.All:
            //        hubContext.Clients.All.sendTransientNotification(model);
            //        break;
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }
    }
}