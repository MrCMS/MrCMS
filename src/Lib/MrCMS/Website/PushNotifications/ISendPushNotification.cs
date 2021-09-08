using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Batching.Entities;
using MrCMS.Entities.People;

namespace MrCMS.Website.PushNotifications
{
    public interface ISendPushNotification
    {
        /// <summary>
        ///     Sends a notification to the specified subscription synchronously
        /// </summary>
        /// <param name="subscription">The MrCMS record of the subscription</param>
        /// <param name="notification">The MrCMS record of the notification</param>
        /// <returns></returns>
        Task<WebPushResult> SendNotification(PushSubscription subscription, PushNotification notification);

        /// <summary>
        ///     Sends a notification to the specified subscription synchronously, passing an object containing the IDs
        /// </summary>
        /// <param name="data">The ids of the notification and subscription</param>
        Task<WebPushResult> SendNotification(SendPushNotificationData data);

        /// <summary>
        ///     Sends a notification to the specified subscription synchronously
        /// </summary>
        /// <param name="subscription">The MrCMS record of the subscription</param>
        /// <param name="body">The body copy of the notification message</param>
        /// <param name="url">If set, will forward the notification to the given URL on click</param>
        /// <param name="title">The title of the notification. If null, will default to the value in the settings</param>
        /// <param name="icon">The icon displayed in the notification. If null, will default to the value in the settings</param>
        /// <param name="badge">The badge displayed for the notification. If null, will default to the value in the settings</param>
        /// <param name="image">The image displayed for the notification. If null, no image will be shown</param>
        /// <returns></returns>
        Task<WebPushResult> SendNotification(PushSubscription subscription, string body, string url = null,
            string title = null, string icon = null, string badge = null, string image = null);

        /// <summary>
        ///     Creates a batch to send the notification to
        /// </summary>
        /// <param name="subscriptions">The MrCMS records of the subscription to batch send to</param>
        /// <param name="body">The body copy of the notification message</param>
        /// <param name="url">If set, will forward the notification to the given URL on click</param>
        /// <param name="title">The title of the notification. If null, will default to the value in the settings</param>
        /// <param name="icon">The icon displayed in the notification. If null, will default to the value in the settings</param>
        /// <param name="badge">The badge displayed for the notification. If null, will default to the value in the settings</param>
        /// <param name="image">The image displayed for the notification. If null, no image will be shown</param>
        /// <returns></returns>
        Task<BatchRun> SendNotificationToSelection(List<PushSubscription> subscriptions, string body, string url = null,
            string title = null, string icon = null, string badge = null, string image = null);

        /// <summary>
        ///     Creates a batch to send the notification to
        /// </summary>
        /// <param name="body">The body copy of the notification message</param>
        /// <param name="url">If set, will forward the notification to the given URL on click</param>
        /// <param name="title">The title of the notification. If null, will default to the value in the settings</param>
        /// <param name="icon">The icon displayed in the notification. If null, will default to the value in the settings</param>
        /// <param name="badge">The badge displayed for the notification. If null, will default to the value in the settings</param>
        /// <param name="image">The image displayed for the notification. If null, no image will be shown</param>
        /// <returns></returns>
        Task<BatchRun> SendNotificationToAll(string body, string url = null,
            string title = null, string icon = null, string badge = null, string image = null);

        /// <summary>
        ///     Creates a batch to send the notification to
        /// </summary>
        /// <param name="roleId">The id of the role to send the notification to</param>
        /// <param name="body">The body copy of the notification message</param>
        /// <param name="url">If set, will forward the notification to the given URL on click</param>
        /// <param name="title">The title of the notification. If null, will default to the value in the settings</param>
        /// <param name="icon">The icon displayed in the notification. If null, will default to the value in the settings</param>
        /// <param name="badge">The badge displayed for the notification. If null, will default to the value in the settings</param>
        /// <param name="image">The image displayed for the notification. If null, no image will be shown</param>
        /// <returns></returns>
        Task<BatchRun> SendNotificationToRole(int roleId, string body, string url = null,
            string title = null, string icon = null, string badge = null, string image = null);
    }
}