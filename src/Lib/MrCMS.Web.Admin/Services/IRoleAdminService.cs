using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IRoleAdminService
    {
        Task<AddRoleResult> AddRole(AddRoleModel model);
        Task<UpdateRoleModel> GetEditModel(int id);
        Task SaveRole(UpdateRoleModel model);
        Task DeleteRole(int id);
        Task<IEnumerable<UserRole>> GetAllRoles();
        Task<IEnumerable<AutoCompleteResult>> Search(string term);
        Task<IEnumerable<string>> GetRolesForPermissions();
    }
}