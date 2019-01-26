using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserTokenManager
    {
        Task<bool> VerifyUserTokenAsync(User user, string tokenProvider, string purpose, string token);
        Task<string> GenerateUserTokenAsync(User user, string tokenProvider, string purpose);
        void RegisterTokenProvider(string providerName, IUserTwoFactorTokenProvider<User> provider);
        Task<string> GetAuthenticationTokenAsync(User user, string loginProvider, string tokenName);
        Task<IdentityResult> SetAuthenticationTokenAsync(User user, string loginProvider, string tokenName, string tokenValue);
        Task<IdentityResult> RemoveAuthenticationTokenAsync(User user, string loginProvider, string tokenName);
    }
}