using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class UserAdminService : IUserAdminService
    {
        private readonly IUserManagementService _service;
        private readonly IMapper _mapper;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IRoleService _roleService;
        private readonly IUserRoleManager _userRoleManager;

        public UserAdminService(IUserManagementService service, IMapper mapper, IPasswordManagementService passwordManagementService, IRoleService roleService, IUserRoleManager userRoleManager)
        {
            _service = service;
            _mapper = mapper;
            _passwordManagementService = passwordManagementService;
            _roleService = roleService;
            _userRoleManager = userRoleManager;
        }
        public async Task<int> AddUser(AddUserModel addUserModel)
        {
            var user = new User();
            var mappedUser = _mapper.Map(addUserModel, user);
            await _service.AddUser(mappedUser);
            _passwordManagementService.SetPassword(mappedUser, addUserModel.Password, addUserModel.ConfirmPassword);
            await _service.SaveUser(mappedUser);
            return mappedUser.Id;
        }

        public UpdateUserModel GetUpdateModel(User user)
        {
            return _mapper.Map<UpdateUserModel>(user);
        }

        public async Task<User> SaveUser(UpdateUserModel model, List<int> roles)
        {
            var user = await _service.GetUser(model.Id);
            _mapper.Map(model, user);

            await _service.SaveUser(user);

            var roleEntities = roles.Select(x => _roleService.GetRole(x)).ToList();
            await _userRoleManager.AssignRoles(user, roleEntities.Select(x => x.Name).ToList());

            return user;
        }

        public async Task DeleteUser(int id)
        {
            await _service.DeleteUser(id);
        }

        public async Task<User> GetUser(int id)
        {
            return await _service.GetUser(id);
        }

        public async Task<bool> IsUniqueEmail(string email, int? id)
        {
            return await _service.IsUniqueEmail(email, id);
        }

        public async Task SetPassword(int id, string password)
        {

            var user = await _service.GetUser(id);
            _passwordManagementService.SetPassword(user, password, password);
            await _service.SaveUser(user);
        }
    }
}