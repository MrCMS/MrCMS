using System;
using MrCMS.Membership.Internals;
using MrCMS.Website;

namespace MrCMS.Membership
{
    public class MrCmsRoleProvider : System.Web.Security.RoleProvider
    {
        private readonly Func<IRoleProvider> _roleProvider;

        private IRoleProvider RoleProvider
        {
            get { return _roleProvider(); }
        }

        public MrCmsRoleProvider()
        {
            _roleProvider = MrCMSApplication.Get<IRoleProvider>;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            return RoleProvider.IsUserInRole(username, roleName);
        }

        public override string[] GetRolesForUser(string username)
        {
            return RoleProvider.GetRolesForUser(username);
        }

        public override void CreateRole(string roleName)
        {
            RoleProvider.CreateRole(roleName);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            return RoleProvider.DeleteRole(roleName, throwOnPopulatedRole);
        }

        public override bool RoleExists(string roleName)
        {
            return RoleProvider.RoleExists(roleName);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            RoleProvider.AddUsersToRoles(usernames, roleNames);
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            RoleProvider.RemoveUsersFromRoles(usernames, roleNames);
        }

        public override string[] GetUsersInRole(string roleName)
        {
            return RoleProvider.GetUsersInRole(roleName);
        }

        public override string[] GetAllRoles()
        {
            return RoleProvider.GetAllRoles();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            return RoleProvider.FindUsersInRole(roleName, usernameToMatch);
        }

        public override string ApplicationName { get; set; }
    }
}
