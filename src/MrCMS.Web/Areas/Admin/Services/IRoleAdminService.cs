using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IRoleAdminService
    {
        Task<AddRoleResult> AddRole(AddRoleModel model);
        UpdateRoleModel GetEditModel(int id);
        Task SaveRole(UpdateRoleModel model);
        Task DeleteRole(int id);
        IEnumerable<Role> GetAllRoles();
        IEnumerable<AutoCompleteResult> Search(string term);
        IEnumerable<string> GetRolesForPermissions();
    }
}