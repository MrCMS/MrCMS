using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public interface IExternalLoginService
    {
        AuthenticationProperties GetExternalConfigurationProperties(string provider, string redirectUrl);

        Task<ExternalLoginCallbackResult> HandleExternalLoginCallback(string returnUrl, string remoteError);
        Task<User> CreateAccountFromExternalLogin(ExternalLoginConfirmationViewModel model);

        //string GetEmail(AuthenticateResult authenticateResult);
        Task<User> GetUserToLogin(ExternalLoginInfo loginInfo);

        Task UpdateClaimsAsync(User user, IEnumerable<Claim> claims);
    }
    public class ExternalLoginCallbackResult
    {
        public bool Success { get; set; }
        public string Error{ get; set; }
        public SignInResult Result { get; set; }
        public ExternalLoginInfo LoginInfo { get; set; }
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }


}