using System;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface ILoginService
    {
        Task<LoginResult> AuthenticateUser(LoginModel loginModel);
    }

    public class LoginService : ILoginService
    {
        private readonly IUserService _userService;
        private readonly IAuthorisationService _authorisationService;
        private readonly IPasswordManagementService _passwordManagementService;

        public LoginService(IUserService userService, IAuthorisationService authorisationService, IPasswordManagementService passwordManagementService)
        {
            _userService = userService;
            _authorisationService = authorisationService;
            _passwordManagementService = passwordManagementService;
        }

        public async Task<LoginResult> AuthenticateUser(LoginModel loginModel)
        {
            string message = null;
            User user = _userService.GetUserByEmail(loginModel.Email);
            if (user == null)
                message = "Incorrect email address.";
            if (user != null && user.IsActive)
            {
                if (_passwordManagementService.ValidateUser(user, loginModel.Password))
                {
                    var guid = CurrentRequestData.UserGuid;
                    await _authorisationService.SetAuthCookie(user, loginModel.RememberMe);
                    CurrentRequestData.CurrentUser = user;
                    EventContext.Instance.Publish<IOnUserLoggedIn, UserLoggedInEventArgs>(
                        new UserLoggedInEventArgs(user, guid));
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