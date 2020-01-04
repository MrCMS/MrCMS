using System.Threading.Tasks;

namespace MrCMS.Website.PushNotifications
{
    public interface IPushNotificationSubscriptionManager
    {
        Task<WebPushResult> CreateSubscription(PushNotificationSubscription subscription);
        Task<WebPushResult> RemoveSubscription(string endpoint);
        string GetServiceWorkerJavaScript();
    }
}