using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public interface ISignInManager
    {
        Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user);
        bool IsSignedIn(ClaimsPrincipal principal);
        Task<bool> CanSignInAsync(User user);
        Task RefreshSignInAsync(User user);
        Task SignInAsync(User user, bool isPersistent = false, string authenticationMethod = null);
        Task SignInAsync(User user, AuthenticationProperties authenticationProperties, string authenticationMethod = null);
        Task SignOutAsync();
        Task<User> ValidateSecurityStampAsync(ClaimsPrincipal principal);
        Task<SignInResult> PasswordSignInAsync(User user, string password, bool isPersistent, bool lockoutOnFailure);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task<SignInResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure);
        Task<bool> IsTwoFactorClientRememberedAsync(User user);
        Task RememberTwoFactorClientAsync(User user);
        Task ForgetTwoFactorClientAsync();
        Task<SignInResult> TwoFactorSignInAsync(string provider, string code, bool isPersistent, bool rememberClient);
        Task<User> GetTwoFactorAuthenticationUserAsync();
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent);
        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey, bool isPersistent, bool bypassTwoFactor);
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        Task<ExternalLoginInfo> GetExternalLoginInfoAsync(string expectedXsrf = null);
        Task<IdentityResult> UpdateExternalAuthenticationTokensAsync(ExternalLoginInfo externalLogin);
        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl, string userId = null);
    }
}