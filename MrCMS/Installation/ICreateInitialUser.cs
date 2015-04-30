using System.Collections.Generic;
using Iesi.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Installation
{
    public interface ICreateInitialUser
    {
        void Create(InstallModel model);
    }

    public class CreateInitialUser : ICreateInitialUser
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IPasswordManagementService _passwordManagementService;

        public CreateInitialUser(IUserService userService, IRoleService roleService,
            IAuthorisationService authorisationService, IPasswordManagementService passwordManagementService)
        {
            _userService = userService;
            _roleService = roleService;
            _authorisationService = authorisationService;
            _passwordManagementService = passwordManagementService;
        }

        public void Create(InstallModel model)
        {
            var user = new User
                           {
                               Email = model.AdminEmail,
                               IsActive = true
                           };
            _passwordManagementService.SetPassword(user, model.AdminPassword, model.ConfirmPassword);

            _userService.AddUser(user);
            CurrentRequestData.CurrentUser = user;

            var adminUserRole = new UserRole
                                    {
                                        Name = UserRole.Administrator
                                    };

            user.Roles = new HashSet<UserRole> { adminUserRole };
            adminUserRole.Users = new HashSet<User> { user };

            _roleService.SaveRole(adminUserRole);
            _authorisationService.Logout();
            _authorisationService.SetAuthCookie(user, true);
        }
    }
}