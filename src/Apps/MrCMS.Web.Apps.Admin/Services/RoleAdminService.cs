using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;
using Newtonsoft.Json;

namespace MrCMS.Web.Apps.Admin.Services
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

        public IEnumerable<UserRole> GetAllRoles()
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

        public AddRoleResult AddRole(AddRoleModel model)
        {
            if (_roleService.GetRoleByName(model.Name) != null)
                return new AddRoleResult(false, string.Format("{0} already exists.", model.Name));
            var role = _mapper.Map<UserRole>(model);
            _roleService.SaveRole(role);
            return new AddRoleResult(true, null);
        }

        public UpdateRoleModel GetEditModel(int id)
        {
            return _mapper.Map<UpdateRoleModel>(GetRole(id));
        }

        private UserRole GetRole(int id)
        {
            return _roleService.GetRole(id);
        }

        public void SaveRole(UpdateRoleModel model)
        {
            var role = GetRole(model.Id);
            _mapper.Map(model, role);
            _roleService.SaveRole(role);
        }

        public void DeleteRole(int id)
        {
            _roleService.DeleteRole(GetRole(id));
        }
    }
}