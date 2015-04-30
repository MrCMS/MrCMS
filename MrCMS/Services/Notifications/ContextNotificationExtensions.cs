using System.Web;

namespace MrCMS.Services.Notifications
{
    public static class ContextNotificationExtensions
    {
        private const string NotificationsDisabledKey = "notifications-disabled";

        public static void DisableNotifications(this HttpContextBase context)
        {
            context.Items[NotificationsDisabledKey] = true;
        }
        public static void EnableNotifications(this HttpContextBase context)
        {
            context.Items.Remove(NotificationsDisabledKey);
        }

        public static bool AreNotificationsDisabled(this HttpContextBase context)
        {
            return context.Items.Contains(NotificationsDisabledKey);
        }
    }
}