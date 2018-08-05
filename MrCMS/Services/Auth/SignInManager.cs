using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MrCMS.Entities.People;

namespace MrCMS.Services.Auth
{
    public class SignInManager : SignInManager<User>, ISignInManager
    {
        public SignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes) : base(userManager,
            contextAccessor, claimsFactory, optionsAccessor, logger, schemes)
        {
        }
    }
}