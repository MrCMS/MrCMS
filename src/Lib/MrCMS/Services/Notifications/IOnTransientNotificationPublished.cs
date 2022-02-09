using MrCMS.Events;

namespace MrCMS.Services.Notifications
{
    public interface IOnTransientNotificationPublished : IEvent<OnTransientNotificationPublishedEventArgs>
    {
    }
}