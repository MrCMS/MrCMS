using System.Threading.Tasks;
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

        public RegistrationService(IUserService userService, IPasswordManagementService passwordManagementService,
                                   IAuthorisationService authorisationService)
        {
            _userService = userService;
            _passwordManagementService = passwordManagementService;
            _authorisationService = authorisationService;
        }

        public async Task<User> RegisterUser(RegisterModel model)
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
            await _authorisationService.SetAuthCookie(user, false);
            EventContext.Instance.Publish<IOnUserRegistered, OnUserRegisteredEventArgs>(new OnUserRegisteredEventArgs(user));
            return user;
        }

        public bool CheckEmailIsNotRegistered(string email)
        {
            return _userService.GetUserByEmail(email) == null;
        }
    }
}