using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using Newtonsoft.Json;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class RoleAdminService : IRoleAdminService
    {
        private readonly IRoleService _roleService;

        public RoleAdminService(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public IEnumerable<UserRole> GetAllRoles()
        {
            return _roleService.GetAllRoles();
        }

        public string GetRolesForPermissions()
        {
            string[] roles = _roleService.GetAllRoles().Select(x => x.Name).ToArray();
            return JsonConvert.SerializeObject(roles);
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            return _roleService.Search(term);
        }

        public AddRoleResult AddRole(UserRole model)
        {
            if (_roleService.GetRoleByName(model.Name) != null)
                return new AddRoleResult(false, string.Format("{0} already exists.", model.Name));
            _roleService.SaveRole(model);
            return new AddRoleResult(true, null);
        }

        public void SaveRole(UserRole role)
        {
            _roleService.SaveRole(role);
        }

        public void DeleteRole(UserRole role)
        {
            _roleService.DeleteRole(role);
        }
    }
}