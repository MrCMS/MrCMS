using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.PushNotifications;

namespace MrCMS.Web.Areas.Admin.Services
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