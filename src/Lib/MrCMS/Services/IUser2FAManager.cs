using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUser2FAManager
    {
        Task<string> GetEmailAsync(User user);
        Task<string> GetPhoneNumberAsync(User user);
        Task<IList<string>> GetValidTwoFactorProvidersAsync(User user);
        Task<bool> VerifyTwoFactorTokenAsync(User user, string tokenProvider, string token);
        Task<string> GenerateTwoFactorTokenAsync(User user, string tokenProvider);
        Task<bool> GetTwoFactorEnabledAsync(User user);
        Task<IdentityResult> SetTwoFactorEnabledAsync(User user, bool enabled);
    }
}