using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IRoleService
    {
        Task SaveRole(UserRole role);
        Task<IList<UserRole>> GetAllRoles();
        Task<UserRole> GetRoleByName(string name);
        Task DeleteRole(UserRole role);
        Task<bool> IsOnlyAdmin(User user);
        Task<IEnumerable<AutoCompleteResult>> Search(string term);
        Task<UserRole> GetRole(int id);
    }
}