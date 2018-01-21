using System.Collections.Generic;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models.Notifications;
using MrCMS.Website;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class PersistentNotificationUIService : IPersistentNotificationUIService
    {
        private readonly ISession _session;
        private readonly IUserLookup _userLookup;

        public PersistentNotificationUIService(ISession session, IUserLookup userLookup)
        {
            _session = session;
            _userLookup = userLookup;
        }

        public IList<NotificationModel> GetNotifications()
        {
            var user = CurrentRequestData.CurrentUser;
            var queryOver = _session.QueryOver<Notification>();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            NotificationModel notificationModelAlias = null;
            return queryOver.SelectList(
                builder =>
                    builder.Select(notification => notification.Message)
                        .WithAlias(() => notificationModelAlias.Message)
                        .Select(notification => notification.CreatedOn)
                        .WithAlias(() => notificationModelAlias.DateValue))
                .OrderBy(notification => notification.CreatedOn).Desc
                .TransformUsing(Transformers.AliasToBean<NotificationModel>())
                .Take(15)
                .Cacheable()
                .List<NotificationModel>();
        }

        public int GetNotificationCount()
        {
            var user = CurrentRequestData.CurrentUser;
            var queryOver = _session.QueryOver<Notification>();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            return queryOver.RowCount();
        }

        public void MarkAllAsRead()
        {
            var user = _userLookup.GetCurrentUser(CurrentRequestData.CurrentContext);
            user.LastNotificationReadDate = CurrentRequestData.Now;
            _session.Transact(session => session.Update(user));
        }
    }
}