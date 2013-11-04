using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface ILoginService
    {
        LoginResult AuthenticateUser(LoginModel loginModel);
    }

    public class LoginService : ILoginService
    {
        private readonly IUserService _userService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IUserEventService _userEventService;

        public LoginService(IUserService userService, IAuthorisationService authorisationService, IPasswordManagementService passwordManagementService, IUserEventService userEventService)
        {
            _userService = userService;
            _authorisationService = authorisationService;
            _passwordManagementService = passwordManagementService;
            _userEventService = userEventService;
        }

        public LoginResult AuthenticateUser(LoginModel loginModel)
        {
            string message = null;
            User user = _userService.GetUserByEmail(loginModel.Email);
            if (user != null && user.IsActive)
            {
                if (_passwordManagementService.ValidateUser(user, loginModel.Password))
                {
                    var guid = CurrentRequestData.UserGuid;
                    _authorisationService.SetAuthCookie(loginModel.Email, loginModel.RememberMe);
                    CurrentRequestData.CurrentUser = user;
                    _userEventService.OnUserLoggedIn(user, guid);
                    return user.IsAdmin
                               ? new LoginResult { Success = true, RedirectUrl = loginModel.ReturnUrl ?? "~/admin" }
                               : new LoginResult { Success = true, RedirectUrl = loginModel.ReturnUrl ?? "~/" };
                }
                message = "Incorrect email or password.";
            }
            return new LoginResult { Success = false, Message = message };
        }
    }
}