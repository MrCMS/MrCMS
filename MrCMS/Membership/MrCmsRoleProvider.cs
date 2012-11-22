using System;
using System.Linq;
using System.Web.Security;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Website;
using Ninject;

namespace MrCMS.Membership
{
    public class MrCmsRoleProvider : RoleProvider
    {
        private readonly Func<IUserService> _userService;

        private IUserService UserService
        {
            get { return _userService.Invoke(); }
        }

        public MrCmsRoleProvider()
        {
            _userService = MrCMSApplication.Get<IUserService>;
        }

        public override bool IsUserInRole(string username, string roleName)
        {

            var user = UserService.GetUserByEmail(username);
            if (user != null)
            {
                if (user.Roles.Any(x => x.Name == roleName))
                    return true;
            }

            return false;
        }

        public override string[] GetRolesForUser(string username)
        {
            var user = UserService.GetUserByEmail(username);

            return user == null || !user.Roles.Any() ? new string[0] : user.Roles.Select(rolename => rolename.Name).ToArray();
        }

        public override void CreateRole(string roleName)
        {
            var role = UserService.GetRoleByName(roleName);
            if (role != null)
            {
                UserService.SaveRole(new UserRole { Name = roleName });
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            var role = UserService.GetRoleByName(roleName);
            if (throwOnPopulatedRole && role.Users.Any())
                throw new Exception("Role " + roleName + " is still populated");

            foreach (User user in role.Users)
                user.Roles.Remove(role);

            UserService.DeleteRole(role);

            return true;
        }

        public override bool RoleExists(string roleName)
        {
            var role = UserService.GetRoleByName(roleName);
            return (role != null);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            foreach (var user in usernames.Select(username => UserService.GetUserByEmail(username)))
            {
                var thisUser = user;
                foreach (var role in roleNames.Select(roleName => UserService.GetRoleByName(roleName)).Where(role => !thisUser.Roles.Contains(role)))
                {
                    user.Roles.Add(role);
                    role.Users.Add(user);
                }
                UserService.SaveUser(user);
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            foreach (var user in usernames.Select(username => UserService.GetUserByEmail(username)))
            {
                var thisUser = user;
                foreach (var role in roleNames.Select(roleName => UserService.GetRoleByName(roleName)).Where(role => thisUser.Roles.Contains(role)))
                {
                    user.Roles.Remove(role);
                    role.Users.Remove(user);
                }
                UserService.SaveUser(user);
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
         var roleByName = UserService.GetRoleByName(roleName);
            return roleByName == null
                       ? new string[0]
                       : roleByName.Users.Select(user => user.Email).ToArray();
        }

        public override string[] GetAllRoles()
        {
            return UserService.GetAllRoles().Select(role => role.Name).ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            var roleByName = UserService.GetRoleByName(roleName);
            return roleByName == null
                       ? new string[0]
                       : roleByName.Users.Where(user => user.Email.Contains(usernameToMatch))
                             .Select(user => user.Email).ToArray();
        }

        public override string ApplicationName { get; set; }
    }
}
