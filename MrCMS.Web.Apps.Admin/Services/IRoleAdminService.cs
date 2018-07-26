using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IRoleAdminService
    {
        AddRoleResult AddRole(UserRole model);
        void SaveRole(UserRole role);
        void DeleteRole(UserRole role);
        IEnumerable<UserRole> GetAllRoles();
        IEnumerable<AutoCompleteResult> Search(string term);
        string GetRolesForPermissions();
    }
}