using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MrCMS.ACL.Rules;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;

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

        public override Task OnConnected()
        {
            User user = _notificationHubService.GetUser(Context.User);
            return user == null || !user.IsAdmin
                ? Groups.Add(Context.ConnectionId, UsersGroup)
                : Groups.Add(Context.ConnectionId, AdminGroup);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            User user = _notificationHubService.GetUser(Context.User);
            return user == null || !user.IsAdmin
                ? Groups.Remove(Context.ConnectionId, UsersGroup)
                : Groups.Remove(Context.ConnectionId, AdminGroup);
        }
    }

    public class MrCMSAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool UserAuthorized(IPrincipal user)
        {
            User currentUser = CurrentRequestData.CurrentUser;
            return base.UserAuthorized(user) && currentUser != null &&
                   currentUser.IsActive &&
                   currentUser.Email != null &&
                   currentUser.Email.Equals(user.Identity.Name, StringComparison.OrdinalIgnoreCase) &&
                   currentUser.CanAccess<AdminAccessACL>(AdminAccessACL.Allowed);
        }
    }
}