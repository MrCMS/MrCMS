using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IRoleService
    {
        Task AddRole(UserRole role);
        Task UpdateRole(UserRole role);
        IEnumerable<UserRole> GetAllRoles();
        UserRole GetRoleByName(string name);
        Task DeleteRole(UserRole role);
        bool IsOnlyAdmin(User user);
        IEnumerable<AutoCompleteResult> Search(string term);
        UserRole GetRole(int id);
    }
}