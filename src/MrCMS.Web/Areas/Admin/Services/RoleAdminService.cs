using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class RoleAdminService : IRoleAdminService
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleAdminService(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        public IEnumerable<Role> GetAllRoles()
        {
            return _roleService.GetAllRoles();
        }

        public IEnumerable<string> GetRolesForPermissions()
        {
            return _roleService.GetAllRoles().Select(x => x.Name);
        }

        public IEnumerable<AutoCompleteResult> Search(string term)
        {
            return _roleService.Search(term);
        }

        public async Task<AddRoleResult> AddRole(AddRoleModel model)
        {
            if (_roleService.GetRoleByName(model.Name) != null)
                return new AddRoleResult(false, string.Format("{0} already exists.", model.Name));
            var role = _mapper.Map<Role>(model);
            await _roleService.AddRole(role);
            return new AddRoleResult(true, null);
        }

        public UpdateRoleModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateRoleModel>(GetRole(id));
        }

        private Role GetRole(int id)
        {
            return _roleService.GetRole(id);
        }

        public async Task SaveRole(UpdateRoleModel model)
        {
            var role = GetRole(model.Id);
            _mapper.Map(model, role);
            await _roleService.UpdateRole(role);
        }

        public async Task DeleteRole(int id)
        {
            await _roleService.DeleteRole(GetRole(id));
        }
    }
}