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
        private readonly IEventContext _eventContext;

        public NotificationPublisher(ISession session, IEventContext eventContext)
        {
            _session = session;
            _eventContext = eventContext;
        }

        public void PublishNotification(string message, PublishType publishType = PublishType.Both)
        {
            var notification = new Notification
                                   {
                                       Message = message,
                                       User = CurrentRequestData.CurrentUser
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
            _eventContext.Publish<IOnPersistentNotificationPublished, OnPersistentNotificationPublishedEventArgs>(
                            new OnPersistentNotificationPublishedEventArgs(notification));
        }

        private void PushNotification(Notification notification)
        {
            _eventContext.Publish<IOnTransientNotificationPublished, OnTransientNotificationPublishedEventArgs>(
                            new OnTransientNotificationPublishedEventArgs(notification));
        }
    }
}