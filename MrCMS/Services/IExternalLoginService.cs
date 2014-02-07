using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace MrCMS.Services
{
    public interface IExternalLoginService
    {
        bool IsLogin(ExternalLoginInfo externalLoginInfo);
        void Login(ExternalLoginInfo externalLoginInfo, AuthenticateResult authenticateResult);
        bool UserExists(string authenticateResult);
        void AssociateLoginToUser(string email, ExternalLoginInfo externalLoginInfo);
        bool RequiresAdditionalFieldsForRegistration();
        void CreateUser(string email, ExternalLoginInfo externalLoginInfo);
        ActionResult RedirectAfterLogin(string email, string returnUrl);
        string GetEmail(AuthenticateResult authenticateResult);
    }
}