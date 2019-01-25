using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using MrCMS.Web.Apps.Admin.Controllers;
using MrCMS.Web.Apps.Admin.Models;
using NHibernate;
using NHibernate.Criterion;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class NotificationAdminService : INotificationAdminService
    {
        private readonly ISession _session;
        private readonly INotificationPublisher _notificationPublisher;

        public NotificationAdminService(ISession session, INotificationPublisher notificationPublisher)
        {
            _session = session;
            _notificationPublisher = notificationPublisher;
        }

        public IPagedList<Notification> Search(NotificationSearchQuery searchQuery)
        {
            var queryOver = _session.QueryOver<Notification>();

            if (!string.IsNullOrWhiteSpace(searchQuery.Message))
            {
                queryOver =
                    queryOver.Where(
                        notification => notification.Message.IsInsensitiveLike(searchQuery.Message, MatchMode.Anywhere));
            }
            if (searchQuery.UserId.HasValue)
            {
                queryOver = queryOver.Where(notification => notification.User.Id == searchQuery.UserId);
            }
            if (searchQuery.From.HasValue)
            {
                queryOver = queryOver.Where(notification => notification.CreatedOn >= searchQuery.From);
            }
            if (searchQuery.To.HasValue)
            {
                queryOver = queryOver.Where(notification => notification.CreatedOn <= searchQuery.To);
            }
            if (searchQuery.NotificationType.HasValue)
            {
                queryOver =
                    queryOver.Where(notification => notification.NotificationType == searchQuery.NotificationType);
            }

            return queryOver.OrderBy(notification => notification.CreatedOn).Desc.Paged(searchQuery.Page);
        }

        public void PushNotification(NotificationModel model)
        {
            _notificationPublisher.PublishNotification(model.Message, model.PublishType, model.NotificationType);
        }

        public List<SelectListItem> GetPublishTypeOptions()
        {
            return Enum.GetValues(typeof(PublishType))
                       .Cast<PublishType>()
                       .BuildSelectItemList(type => type.ToString(), emptyItem: null);
        }
        public List<SelectListItem> GetNotificationTypeOptions(bool includeAnyOption)
        {
            return Enum.GetValues(typeof (NotificationType))
                       .Cast<NotificationType>()
                       .BuildSelectItemList(type => type.ToString().BreakUpString(), type => type.ToString(),
                                            emptyItemText: includeAnyOption ? "Any" : null);
        }

        public void Delete(Notification notification)
        {
            _session.Transact(session => session.Delete(notification));
        }
    }
}