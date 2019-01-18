namespace MrCMS.Website.PushNotifications
{
    public interface IPushNotificationSubscriptionManager
    {
        WebPushResult CreateSubscription(PushNotificationSubscription subscription);
        WebPushResult RemoveSubscription(string endpoint);
        string GetServiceWorkerJavaScript();
    }
}