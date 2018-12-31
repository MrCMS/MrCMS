using System.Web;
using FluentNHibernate.Conventions;
using Microsoft.AspNetCore.Http;

namespace MrCMS.Services.Notifications
{
    public static class ContextNotificationExtensions
    {
        private const string NotificationsDisabledKey = "notifications-disabled";

        public static void DisableNotifications(this HttpContext context)
        {
            context.Items[NotificationsDisabledKey] = true;
        }
        public static void EnableNotifications(this HttpContext context)
        {
            context.Items.Remove(NotificationsDisabledKey);
        }

        public static bool AreNotificationsDisabled(this HttpContext context)
        {
            return context.Items.ContainsKey(NotificationsDisabledKey);
        }
    }
}