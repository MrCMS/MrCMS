using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MrCMS.Entities.People;
using MrCMS.Services;

namespace MrCMS.Web.Apps.Admin.Hubs
{
    public class NotificationHub : Hub
    {
        public const string UsersGroup = "Users";
        public const string AdminGroup = "Admins";
        private readonly IUserLookup _userLookup;
        private readonly IUserRoleManager _userRoleManager;

        public NotificationHub(IUserLookup userLookup, IUserRoleManager userRoleManager)
        {
            _userLookup = userLookup;
            _userRoleManager = userRoleManager;
        }

        public override async Task OnConnectedAsync()
        {
            User user = _userLookup.GetCurrentUser(Context.User);
            if (user == null || !await _userRoleManager.IsAdmin(user))
                await Groups.AddToGroupAsync(Context.ConnectionId, UsersGroup);
            else
                await Groups.AddToGroupAsync(Context.ConnectionId, AdminGroup);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            User user = _userLookup.GetCurrentUser(Context.User);
            if (user == null || !await _userRoleManager.IsAdmin(user))
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, UsersGroup);
            else
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, AdminGroup);
        }
    }
}