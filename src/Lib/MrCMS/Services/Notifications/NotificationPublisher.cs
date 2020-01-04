using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Website;

namespace MrCMS.Services.Notifications
{
    public class NotificationPublisher : INotificationPublisher
    {
        private readonly IRepository<Notification> _repository;
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IEventContext _eventContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public NotificationPublisher(IRepository<Notification> repository, IGetCurrentUser getCurrentUser, IEventContext eventContext, IHttpContextAccessor contextAccessor)
        {
            _repository = repository;
            _getCurrentUser = getCurrentUser;
            _eventContext = eventContext;
            _contextAccessor = contextAccessor;
        }

        public async Task PublishNotification(string message, PublishType publishType = PublishType.Both,
            NotificationType notificationType = NotificationType.All)
        {
            if (_contextAccessor.HttpContext.AreNotificationsDisabled())
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
                    await PushNotification(notification);
                    break;
                case PublishType.Persistent:
                    await SaveNotification(notification);
                    break;
                case PublishType.Both:
                    await SaveNotification(notification);
                    await PushNotification(notification);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("publishType");
            }
        }

        private async Task SaveNotification(Notification notification)
        {
            await _repository.Add(notification);
            await _eventContext.Publish<IOnPersistentNotificationPublished, OnPersistentNotificationPublishedEventArgs>(
                            new OnPersistentNotificationPublishedEventArgs(notification));
        }

        private async Task PushNotification(Notification notification)
        {
            await _eventContext.Publish<IOnTransientNotificationPublished, OnTransientNotificationPublishedEventArgs>(
                                       new OnTransientNotificationPublishedEventArgs(notification));
        }
    }
}