using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Web.Admin.Hubs
{
    public class NotificationHub : Hub
    {
        public const string UsersGroup = "Users";
        public const string AdminGroup = "Admins";
        private readonly IUserLookup _userLookup;

        public NotificationHub(IUserLookup userLookup)
        {
            _userLookup = userLookup;
        }

        public override Task OnConnectedAsync()
        {
            User user = _userLookup.GetCurrentUser(Context.User);
            return user == null || !user.IsAdmin
                ? Groups.AddToGroupAsync(Context.ConnectionId, UsersGroup)
                : Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            User user = _userLookup.GetCurrentUser(Context.User);
            return user == null || !user.IsAdmin
                ? Groups.RemoveFromGroupAsync(Context.ConnectionId, UsersGroup)
                : Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminGroup);
        }
    }
}