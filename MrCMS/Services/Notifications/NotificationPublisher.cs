using System;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly ISession _session;

        public NotificationPublisher(ISession session)
        {
            _session = session;
        }

        public void PublishNotification(string message, PublishType publishType = PublishType.Both, NotificationType notificationType = NotificationType.All)
        {
            var notification = new Notification
                                   {
                                       Message = message,
                                       User = CurrentRequestData.CurrentUser,
                                       NotificationType = notificationType
                                   };
            switch (publishType)
            {
                case PublishType.Transient:
                    PushNotification(notification);
                    break;
                case PublishType.Persistent:
                    SaveNotification(notification);
                    break;
                case PublishType.Both:
                    SaveNotification(notification);
                    PushNotification(notification);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("publishType");
            }
        }

        private void SaveNotification(Notification notification)
        {
            _session.Transact(session => session.Save(notification));
            EventContext.Instance.Publish<IOnPersistentNotificationPublished, OnPersistentNotificationPublishedEventArgs>(
                            new OnPersistentNotificationPublishedEventArgs(notification));
        }

        private void PushNotification(Notification notification)
        {
            EventContext.Instance.Publish<IOnTransientNotificationPublished, OnTransientNotificationPublishedEventArgs>(
                            new OnTransientNotificationPublishedEventArgs(notification));
        }
    }
}