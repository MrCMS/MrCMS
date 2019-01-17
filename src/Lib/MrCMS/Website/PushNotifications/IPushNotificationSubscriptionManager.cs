namespace MrCMS.Website.PushNotifications
{
    public interface IPushNotificationSubscriptionManager
    {
        PushSubscriptionResult CreateSubscription(PushNotificationSubscription subscription);
        PushSubscriptionResult RemoveSubscription(string endpoint);
        string GetServiceWorkerJavaScript();
    }
}