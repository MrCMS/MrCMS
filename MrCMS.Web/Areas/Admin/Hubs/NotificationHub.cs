using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MrCMS.ACL.Rules;
using MrCMS.Entities.Notifications;
using MrCMS.Entities.People;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Hubs
{
    [MrCMSAuthorize]
    [HubName("notifications")]
    public class NotificationHub : Hub
    {
        public const string UsersGroup = "Users";
        public const string AdminGroup = "Admins";
        private readonly INotificationHubService _notificationHubService;

        public NotificationHub(INotificationHubService notificationHubService)
        {
            _notificationHubService = notificationHubService;
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            var user = _notificationHubService.GetUser(Context.User);
            return user == null || !user.IsAdmin
                       ? Groups.Add(Context.ConnectionId, UsersGroup)
                       : Groups.Add(Context.ConnectionId, AdminGroup);
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            var user = _notificationHubService.GetUser(Context.User);
            return user == null || !user.IsAdmin
                       ? Groups.Remove(Context.ConnectionId, UsersGroup)
                       : Groups.Remove(Context.ConnectionId, AdminGroup);
        }
    }

    public interface INotificationHubService
    {
        User GetUser(IPrincipal user);
    }

    public class NotificationHubService : INotificationHubService
    {
        private readonly ISession _session;

        public NotificationHubService(ISession session)
        {
            _session = session;
        }

        public User GetUser(IPrincipal user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Identity.Name))
                return null;
            return _session.QueryOver<User>()
                    .Where(u => u.Email == user.Identity.Name)
                    .Take(1)
                    .Cacheable()
                    .SingleOrDefault();
        }
    }

    public class MrCMSAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool UserAuthorized(IPrincipal user)
        {
            var currentUser = CurrentRequestData.CurrentUser;
            return base.UserAuthorized(user) && currentUser != null &&
                   currentUser.IsActive &&
                   currentUser.Email != null &&
                   currentUser.Email.Equals(user.Identity.Name, StringComparison.OrdinalIgnoreCase) &&
                   currentUser.CanAccess<AdminAccessACL>(AdminAccessACL.Allowed);
        }
    }

}