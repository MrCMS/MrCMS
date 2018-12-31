using System.Threading.Tasks;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public class LoginService : ILoginService
    {
        private readonly IPasswordManagementService _passwordManagementService;
        private readonly IGetVerifiedUserResult _getVerifiedUserResult;
        private readonly IUserLookup _userLookup;

        public LoginService(IUserLookup userLookup,
            IPasswordManagementService passwordManagementService,
            IGetVerifiedUserResult getVerifiedUserResult)
        {
            _userLookup = userLookup;
            _passwordManagementService = passwordManagementService;
            _getVerifiedUserResult = getVerifiedUserResult;
        }

        public LoginResult AuthenticateUser(LoginModel loginModel)
        {
            if (string.IsNullOrWhiteSpace(loginModel.ReturnUrl))
                loginModel.ReturnUrl = null;

            var user = _userLookup.GetUserByEmail(loginModel.Email);

            if (user != null && _passwordManagementService.ValidateUser(user, loginModel.Password))
                return _getVerifiedUserResult.GetResult(user, loginModel.ReturnUrl);

            return new LoginResult
            {
                User = user,
                Status = LoginStatus.Failure,
                Message = "Incorrect email or password."
            };
        }
    }
}