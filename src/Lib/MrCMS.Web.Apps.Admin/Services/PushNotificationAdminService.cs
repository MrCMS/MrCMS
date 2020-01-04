using MrCMS.Batching.Services;
using MrCMS.Entities.People;
using MrCMS.Web.Apps.Admin.Models;

using System.Linq;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;
using MrCMS.Website.PushNotifications;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class PushNotificationAdminService : IPushNotificationAdminService
    {
        private readonly ISendPushNotification _sendPushNotification;

        public PushNotificationAdminService(ISendPushNotification sendPushNotification)
        {
            _sendPushNotification = sendPushNotification;
        }

        public bool PushToAll(PushNotificationModel model)
        {
            var run = _sendPushNotification.SendNotificationToAll(model.Body, model.Url, model.Title, model.Icon, model.Badge);
            return run != null;
        }
    }

}