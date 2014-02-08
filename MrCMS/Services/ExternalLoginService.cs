using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly UserManager<User> _userManager;

        public ExternalLoginService(UserManager<User> userManager, IAuthorisationService authorisationService)
        {
            _userManager = userManager;
            _authorisationService = authorisationService;
        }

        public bool IsLogin(ExternalLoginInfo externalLoginInfo)
        {
            return _userManager.Find(externalLoginInfo.Login) != null;
        }

        public void Login(ExternalLoginInfo externalLoginInfo, AuthenticateResult authenticateResult)
        {
            User user = _userManager.Find(externalLoginInfo.Login);
            _authorisationService.SetAuthCookie(user, false);
            _authorisationService.UpdateClaims(user, authenticateResult.Identity.Claims);
        }

        public bool UserExists(string email)
        {
            return _userManager.FindByName(email) != null;
        }

        public string GetEmail(AuthenticateResult authenticateResult)
        {
            if (authenticateResult != null && authenticateResult.Identity != null)
            {
                IEnumerable<Claim> claims = authenticateResult.Identity.Claims;
                if (claims != null)
                {
                    Claim emailClaim = claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                    if (emailClaim != null)
                        return emailClaim.Value;
                }
            }
            return null;
        }

        public void AssociateLoginToUser(string email, ExternalLoginInfo externalLoginInfo)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;
            var user = _userManager.FindByName(email);
            if (user == null)
                return;
            _userManager.AddLogin(user.OwinId, externalLoginInfo.Login);
        }

        public bool RequiresAdditionalFieldsForRegistration()
        {
            //TODO: wire in framework to allow extra fields to be added

            return false;
        }

        public void CreateUser(string email, ExternalLoginInfo externalLoginInfo)
        {
            var user = new User { Email = email, IsActive = true };
            _userManager.Create(user);
            _userManager.AddLogin(user.OwinId, externalLoginInfo.Login);
        }

        public ActionResult RedirectAfterLogin(string email, string returnUrl)
        {
            var user = _userManager.FindByName(email);
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return new RedirectResult(returnUrl);
            return user.IsAdmin ? new RedirectResult("~/admin") : new RedirectResult("~");
        }
    }
}