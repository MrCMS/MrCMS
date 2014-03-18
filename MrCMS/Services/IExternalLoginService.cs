using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MrCMS.Services
{
    public interface IExternalLoginService
    {
        Task<bool> IsLoginAsync(ExternalLoginInfo externalLoginInfo);
        Task LoginAsync(ExternalLoginInfo externalLoginInfo, AuthenticateResult authenticateResult);
        Task<bool> UserExistsAsync(string authenticateResult);
        Task AssociateLoginToUserAsync(string email, ExternalLoginInfo externalLoginInfo);
        bool RequiresAdditionalFieldsForRegistration();
        Task CreateUserAsync(string email, ExternalLoginInfo externalLoginInfo);
        Task<RedirectResult> RedirectAfterLogin(string email, string returnUrl);
        string GetEmail(AuthenticateResult authenticateResult);
    }
}