using System.Collections.Generic;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
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