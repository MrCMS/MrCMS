using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;

namespace MrCMS.Web.Apps.Core.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserService _userService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IUserEventService _userEventService;

        public RegistrationService(IUserService userService, IPasswordManagementService passwordManagementService,
                                   IAuthorisationService authorisationService, IUserEventService userEventService)
        {
            _userService = userService;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
            _userEventService = userEventService;
        }

        public User RegisterUser(RegisterModel model)
        {
            var user = new User
                           {
                               FirstName = model.FirstName,
                               LastName = model.LastName,
                               Email = model.Email,
                               IsActive = true
                           };
            _passwordManagementService.SetPassword(user, model.Password, model.ConfirmPassword);
            _userService.AddUser(user);
            _authorisationService.SetAuthCookie(user, false);
            _userEventService.OnUserRegistered(user);
            return user;
        }

        public bool CheckEmailIsNotRegistered(string email)
        {
            return _userService.GetUserByEmail(email) == null;
        }
    }
}