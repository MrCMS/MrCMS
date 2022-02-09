using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Installation.Models;
using MrCMS.Services;

namespace MrCMS.Installation.Services
{
    public class CreateInitialUser : ICreateInitialUser
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IRoleService _roleService;
        private readonly IPasswordManagementService _passwordManagementService;

        public CreateInitialUser(IUserManagementService userManagementService, IRoleService roleService,
            IPasswordManagementService passwordManagementService)
        {
            _userManagementService = userManagementService;
            _roleService = roleService;
            _passwordManagementService = passwordManagementService;
        }

        public async Task Create(InstallModel model)
        {
            var user = new User
            {
                Email = model.AdminEmail,
                IsActive = true
            };
            _passwordManagementService.SetPassword(user, model.AdminPassword, model.ConfirmPassword);

            await _userManagementService.AddUser(user);

            var adminUserRole = new UserRole
            {
                Name = UserRole.Administrator
            };

            user.Roles = new HashSet<UserRole> { adminUserRole };
            adminUserRole.Users = new HashSet<User> { user };

            await _roleService.SaveRole(adminUserRole);
        }
    }
}