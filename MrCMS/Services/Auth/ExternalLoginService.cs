using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class ExternalLoginService : IExternalLoginService
    {
        private readonly IAuthorisationService _authorisationService;
        private readonly IUserManager _userManager;

        public ExternalLoginService(IUserManager userManager, IAuthorisationService authorisationService)
        {
            _userManager = userManager;
            _authorisationService = authorisationService;
        }

        public string GetEmail(AuthenticateResult authenticateResult)
        {
            if (authenticateResult?.Identity != null)
            {
                IEnumerable<Claim> claims = authenticateResult.Identity.Claims;
                Claim emailClaim = claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Email);
                if (emailClaim != null)
                    return emailClaim.Value;
            }
            return null;
        }

        public async Task<User> GetUserToLogin(ExternalLoginInfo loginInfo)
        {
            var user = await _userManager.FindAsync(loginInfo.Login);
            if (user != null)
                return user;
            user = await _userManager.FindByNameAsync(loginInfo.Email);
            if (user == null)
            {
                user = new User { Email = loginInfo.Email, IsActive = true };
                await _userManager.CreateAsync(user);
            }
            await _userManager.AddLoginAsync(user.Id, loginInfo.Login);
            return user;
        }

        public async Task UpdateClaimsAsync(User user, IEnumerable<Claim> claims)
        {
            var existingClaims = (await _userManager.GetClaimsAsync(user.Id)).ToList();
            var list = claims as IList<Claim> ?? claims.ToList();
            var newClaims =
                list.Where(claim => existingClaims.All(c => c.Type != claim.Type));
            var updatedClaims =
                list.Where(claim => existingClaims.Any(c => c.Type == claim.Type && c.Value != claim.Value));
            foreach (var newClaim in newClaims)
                await _userManager.AddClaimAsync(user.Id, newClaim);
            foreach (var updatedClaim in updatedClaims)
            {
                var existing = existingClaims.First(c => c.Type == updatedClaim.Type);
                await _userManager.RemoveClaimAsync(user.Id, existing);
                await _userManager.AddClaimAsync(user.Id, updatedClaim);
            }
        }
    }
}