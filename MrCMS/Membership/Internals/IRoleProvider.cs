namespace MrCMS.Membership.Internals
{
    public interface IRoleProvider
    {
        bool IsUserInRole(string username, string roleName);
        string[] GetRolesForUser(string username);
        void CreateRole(string roleName);
        bool DeleteRole(string roleName, bool throwOnPopulatedRole);
        bool RoleExists(string roleName);
        void AddUsersToRoles(string[] usernames, string[] roleNames);
        void RemoveUsersFromRoles(string[] usernames, string[] roleNames);
        string[] GetUsersInRole(string roleName);
        string[] GetAllRoles();
        string[] FindUsersInRole(string roleName, string usernameToMatch);
    }
}