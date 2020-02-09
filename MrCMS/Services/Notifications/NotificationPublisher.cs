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
        private readonly IGetCurrentUser _getCurrentUser;

        public NotificationPublisher(ISession session,IGetCurrentUser getCurrentUser)
        {
            _session = session;
            _getCurrentUser = getCurrentUser;
        }

        public void PublishNotification(string message, PublishType publishType = PublishType.Both, NotificationType notificationType = NotificationType.All)
        {
            if (CurrentRequestData.CurrentContext.AreNotificationsDisabled())
                return;

            var notification = new Notification
                                   {
                                       Message = message,
                                       User = _getCurrentUser.Get(),
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