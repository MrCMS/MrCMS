using System.Web.Security;

namespace MrCMS.Services
{
    public class AuthorisationService : IAuthorisationService
    {
        public void SetAuthCookie(string email, bool rememberMe)
        {
            FormsAuthentication.SetAuthCookie(email, rememberMe);
        }

        public void Logout()
        {
            FormsAuthentication.SignOut();
        }
    }
}