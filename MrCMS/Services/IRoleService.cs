using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IRoleService
    {
        void Add(UserRole role);
        void Update(UserRole role);
        IEnumerable<UserRole> GetAllRoles();
        UserRole GetRoleByName(string name);
        void DeleteRole(UserRole role);
        bool IsOnlyAdmin(User user);
        IEnumerable<AutoCompleteResult> Search(string term);
        UserRole GetRole(int id);
    }
}