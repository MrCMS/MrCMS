using MrCMS.Events;

namespace MrCMS.Services.Notifications
{
    public interface IOnPersistentNotificationPublished : IEvent<OnPersistentNotificationPublishedEventArgs>
    {
    }
}