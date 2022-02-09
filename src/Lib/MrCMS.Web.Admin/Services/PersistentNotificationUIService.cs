using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Admin.Models.Notifications;
using NHibernate;
using NHibernate.Transform;

namespace MrCMS.Web.Admin.Services
{
    public class PersistentNotificationUIService : IPersistentNotificationUIService
    {
        private readonly ISession _session;
        private readonly IGetCurrentUser _getCurrentUser;

        public PersistentNotificationUIService(ISession session,  IGetCurrentUser getCurrentUser)
        {
            _session = session;
            _getCurrentUser = getCurrentUser;
        }

        public async Task<IList<NotificationModel>> GetNotifications()
        {
            var user = await _getCurrentUser.Get();
            var queryOver = _session.QueryOver<Notification>();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            NotificationModel notificationModelAlias = null;
            return await queryOver.SelectList(
                builder =>
                    builder.Select(notification => notification.Message)
                        .WithAlias(() => notificationModelAlias.Message)
                        .Select(notification => notification.CreatedOn)
                        .WithAlias(() => notificationModelAlias.DateValue))
                .OrderBy(notification => notification.CreatedOn).Desc
                .TransformUsing(Transformers.AliasToBean<NotificationModel>())
                .Take(15)
                .Cacheable()
                .ListAsync<NotificationModel>();
        }

        public async Task<int> GetNotificationCount()
        {
            var user = await _getCurrentUser.Get();
            var queryOver = _session.QueryOver<Notification>();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            return await queryOver.RowCountAsync();
        }

        public async Task MarkAllAsRead()
        {
            var user = await _getCurrentUser.Get();
            user.LastNotificationReadDate = DateTime.UtcNow;
            await _session.TransactAsync(session => session.UpdateAsync(user));
        }
    }
}