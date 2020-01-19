using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IRoleService
    {
        Task AddRole(Role role);
        Task UpdateRole(Role role);
        IEnumerable<Role> GetAllRoles();
        Role GetRoleByName(string name);
        Task DeleteRole(Role role);
        bool IsOnlyAdmin(User user);
        IEnumerable<AutoCompleteResult> Search(string term);
        Role GetRole(int id);
    }
}