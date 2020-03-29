using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Notifications;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models.Notifications;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class PersistentNotificationUIService : IPersistentNotificationUIService
    {
        private readonly IRepository<Notification> _repository;
        private readonly IGlobalRepository<User> _userRepository;
        private readonly IGetCurrentUser _getCurrentUser;

        public PersistentNotificationUIService(IRepository<Notification> repository, 
            IGlobalRepository<User> userRepository,
            IGetCurrentUser getCurrentUser)
        {
            _repository = repository;
            _userRepository = userRepository;
            _getCurrentUser = getCurrentUser;
        }

        public IList<NotificationModel> GetNotifications()
        {
            var user = _getCurrentUser.Get();
            var queryOver = _repository.Query();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            //NotificationModel notificationModelAlias = null;
            return queryOver.Select(
                    notification => new NotificationModel
                    {
                        Message = notification.Message,
                        DateValue = notification.CreatedOn
                    })
                .OrderByDescending(notification => notification.DateValue)
                .Take(15)
                .ToList();
        }

        public int GetNotificationCount()
        {
            var user = _getCurrentUser.Get();
            var queryOver = _repository.Query();

            if (user.LastNotificationReadDate.HasValue)
                queryOver = queryOver.Where(notification => notification.CreatedOn >= user.LastNotificationReadDate);

            return queryOver.Count();
        }

        public async Task MarkAllAsRead()
        {
            var user = _getCurrentUser.Get();
            user.LastNotificationReadDate = DateTime.UtcNow;
            await _userRepository.Update(user);
        }
    }
}