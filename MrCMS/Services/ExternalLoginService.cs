using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
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
        private readonly UserManager<User, int> _userManager;

        public ExternalLoginService(UserManager<User, int> userManager, IAuthorisationService authorisationService)
        {
            _userManager = userManager;
            _authorisationService = authorisationService;
        }

        public async Task<bool> IsLoginAsync(ExternalLoginInfo externalLoginInfo)
        {
            return (await _userManager.FindAsync(externalLoginInfo.Login)) != null;
        }

        public async Task LoginAsync(ExternalLoginInfo externalLoginInfo, AuthenticateResult authenticateResult)
        {
            User user = await _userManager.FindAsync(externalLoginInfo.Login);
            await _authorisationService.SetAuthCookie(user, false);
            await _authorisationService.UpdateClaimsAsync(user, authenticateResult.Identity.Claims);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _userManager.FindByNameAsync(email) != null;
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

        public async Task AssociateLoginToUserAsync(string email, ExternalLoginInfo externalLoginInfo)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;
            var user = await _userManager.FindByNameAsync(email);
            if (user == null)
                return;
            await _userManager.AddLoginAsync(user.Id, externalLoginInfo.Login);
        }

        public bool RequiresAdditionalFieldsForRegistration()
        {
            //TODO: wire in framework to allow extra fields to be added

            return false;
        }

        public async Task CreateUserAsync(string email, ExternalLoginInfo externalLoginInfo)
        {
            var user = new User { Email = email, IsActive = true };
            await _userManager.CreateAsync(user);
            await _userManager.AddLoginAsync(user.Id, externalLoginInfo.Login);
        }

        public async Task<RedirectResult> RedirectAfterLogin(string email, string returnUrl)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (!string.IsNullOrWhiteSpace(returnUrl))
                return new RedirectResult(returnUrl);
            return user.IsAdmin ? new RedirectResult("~/admin") : new RedirectResult("~");
        }
    }
}