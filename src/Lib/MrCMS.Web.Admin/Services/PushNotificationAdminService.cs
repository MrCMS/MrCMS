using MrCMS.Entities.People;
using NHibernate;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using MrCMS.Website.PushNotifications;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class PushNotificationAdminService : IPushNotificationAdminService
    {
        private readonly ISendPushNotification _sendPushNotification;
        private readonly ISession _session;

        public PushNotificationAdminService(ISendPushNotification sendPushNotification, ISession session)
        {
            _sendPushNotification = sendPushNotification;
            _session = session;
        }

        public async Task<bool> PushToAll(PushNotificationModel model)
        {
            var run = await _sendPushNotification.SendNotificationToAll(model.Body, model.Url, model.Title, model.Icon,
                model.Badge, model.Image);
            return run != null;
        }

        public async Task<bool> PushToRole(PushToRoleNotificationModel model)
        {
            var run = await _sendPushNotification.SendNotificationToRole(model.RoleId.GetValueOrDefault(), model.Body,
                model.Url, model.Title, model.Icon, model.Badge, model.Image);
            return run != null;
        }

        public async Task<IPagedList<PushNotification>> Search(PushNotificationSearchModel searchModel)
        {
            var query = _session.Query<PushNotification>();

            query = query.OrderByDescending(x => x.CreatedOn);

            return await query.PagedAsync(searchModel.PageNumber);
        }
    }
}