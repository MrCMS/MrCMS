using System.Collections.Generic;
using System.Linq;
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

        public UserAdminService(IUserManagementService service, IMapper mapper, IPasswordManagementService passwordManagementService, IRoleService roleService)
        {
            _service = service;
            _mapper = mapper;
            _passwordManagementService = passwordManagementService;
            _roleService = roleService;
        }
        public void AddUser(User user)
        {
            _service.AddUser(user);
        }

        public UpdateUserModel GetUpdateModel(User user)
        {
            return _mapper.Map<UpdateUserModel>(user);
        }

        public User SaveUser(UpdateUserModel model, List<int> roles)
        {
            var user = _service.GetUser(model.Id);
            _mapper.Map(model, user);

            var userRoles = _roleService.GetAllRoles().ToList();
            userRoles.ForEach(x => x.Users.Remove(user));

            var rolesToSet = roles.Select(id => _roleService.GetRole(id)).Where(x => x != null).ToHashSet();
            rolesToSet.ForEach(role => role.Users.Add(user));
            user.Roles = rolesToSet;
            _service.SaveUser(user);
            return user;
        }

        public void DeleteUser(int id)
        {
            _service.DeleteUser(id);
        }

        public User GetUser(int id)
        {
            return _service.GetUser(id);
        }

        public bool IsUniqueEmail(string email, int? id)
        {
            return _service.IsUniqueEmail(email, id);
        }

        public void SetPassword(int id, string password)
        {

            var user = _service.GetUser(id);
            _passwordManagementService.SetPassword(user, password, password);
            _service.SaveUser(user);
        }
    }
}