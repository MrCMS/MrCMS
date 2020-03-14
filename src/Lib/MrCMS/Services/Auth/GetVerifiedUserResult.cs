using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models.Auth;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class GetVerifiedUserResult : IGetVerifiedUserResult
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly IUserRoleManager _userRoleManager;

        public GetVerifiedUserResult(ISystemConfigurationProvider configurationProvider, IUserRoleManager userRoleManager)
        {
            _configurationProvider = configurationProvider;
            _userRoleManager = userRoleManager;
        }

        public async Task<LoginResult> GetResult(User user, string returnUrl)
        {
            if (!user.IsActive)
                return new LoginResult { Status = LoginStatus.LockedOut, Message = "User is locked out." };

            var loginModelReturnUrl = returnUrl;
            if (!IsRelativeRedirect(loginModelReturnUrl))  //stop redirect to other sites
            {
                loginModelReturnUrl = null;
            }

            var redirectUrl = loginModelReturnUrl ?? (await _userRoleManager.IsAdmin(user) ? "~/admin" : "/");

            var securitySettings = await _configurationProvider.GetSystemSettings<SecuritySettings>();
            var roleSettings = await _configurationProvider.GetSystemSettings<AuthRoleSettings>();
            if (securitySettings.TwoFactorAuthEnabled && user.UserToRoles.Any(role => roleSettings.TwoFactorAuthRoles.Contains(role.UserRoleId)))
                return new LoginResult
                {
                    User = user,
                    Status = LoginStatus.TwoFactorRequired,
                    ReturnUrl = redirectUrl
                };

            return new LoginResult
            {
                User = user,
                Status = LoginStatus.Success,
                ReturnUrl = redirectUrl
            };
        }

        private bool IsRelativeRedirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;
            return Uri.IsWellFormedUriString(url, UriKind.Relative) && !url.Trim().StartsWith("//");
        }
    }
}