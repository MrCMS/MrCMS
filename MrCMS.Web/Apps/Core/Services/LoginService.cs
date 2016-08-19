using System.Threading.Tasks;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;
using MrCMS.Website;

namespace MrCMS.Web.Apps.Core.Services
{
    public class LoginService : ILoginService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IUserLookup _userLookup;

        public LoginService(IUserLookup userLookup, IAuthorisationService authorisationService,
            IPasswordManagementService passwordManagementService)
        {
            _userLookup = userLookup;
            _authorisationService = authorisationService;
            _passwordManagementService = passwordManagementService;
        }

        public async Task<LoginResult> AuthenticateUser(LoginModel loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.ReturnUrl))
                loginModel.ReturnUrl = null;
            string message = null;

            var user = _userLookup.GetUserByEmail(loginModel.Email);
            if (user == null)
                return new LoginResult { Success = false, Message = "Incorrect email address" };
            if (_passwordManagementService.ValidateUser(user, loginModel.Password) && user.IsActive)
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
            return new LoginResult { Success = false, Message = "Incorrect email or password." };
        }
    }
}