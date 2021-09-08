using System.Threading.Tasks;

namespace MrCMS.Website.PushNotifications
{
    public interface IPushNotificationSubscriptionManager
    {
        Task<WebPushResult> CreateOrUpdateSubscription(PushNotificationSubscription subscription);
        Task<WebPushResult> RemoveSubscription(string endpoint);
        Task<string> GetServiceWorkerJavaScript();
    }
}