using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public interface IExternalLoginService
    {
        string GetEmail(AuthenticateResult authenticateResult);
        Task<User> GetUserToLogin(ExternalLoginInfo loginInfo);

        Task UpdateClaimsAsync(User user, IEnumerable<Claim> claims);
    }
}