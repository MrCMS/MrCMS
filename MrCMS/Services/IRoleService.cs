using System.Collections.Generic;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IRoleService
    {
        void SaveRole(UserRole role);
        UserRole GetRole(int id);
        IEnumerable<UserRole> GetAllRoles();
        UserRole GetRoleByName(string name);
        void DeleteRole(UserRole role);
        bool IsOnlyAdmin(User user);
    }
}