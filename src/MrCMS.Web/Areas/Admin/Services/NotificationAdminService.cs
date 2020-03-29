using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Notifications;
using MrCMS.Helpers;
using MrCMS.Services.Notifications;
using MrCMS.Web.Areas.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class NotificationAdminService : INotificationAdminService
    {
        private readonly IRepository<Notification> _repository;
        private readonly INotificationPublisher _notificationPublisher;

        public NotificationAdminService(IRepository<Notification> repository, INotificationPublisher notificationPublisher)
        {
            _repository = repository;
            _notificationPublisher = notificationPublisher;
        }

        public IPagedList<Notification> Search(NotificationSearchQuery searchQuery)
        {
            IQueryable<Notification> queryOver = _repository.Query().Include(x => x.User);

            if (!string.IsNullOrWhiteSpace(searchQuery.Message))
            {
                queryOver =
                    queryOver.Where(
                        notification => EF.Functions.Like(notification.Message, $"%{searchQuery.Message}%"));
            }
            if (searchQuery.UserId.HasValue)
            {
                queryOver = queryOver.Where(notification => notification.UserId == searchQuery.UserId);
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

            return queryOver.OrderByDescending(notification => notification.CreatedOn).ToPagedList(searchQuery.Page);
        }

        public async Task PushNotification(NotificationModel model)
        {
            await _notificationPublisher.PublishNotification(model.Message, model.PublishType, model.NotificationType);
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

        public async Task Delete(Notification notification)
        {
            await _repository.Delete(notification);
        }
    }
}