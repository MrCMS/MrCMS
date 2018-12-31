using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserEmailManager
    {
        Task<IdentityResult> SetEmailAsync(User user, string email);
        Task UpdateNormalizedEmailAsync(User user);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);
        Task<bool> IsEmailConfirmedAsync(User user);
        Task<string> GenerateChangeEmailTokenAsync(User user, string newEmail);
        Task<IdentityResult> ChangeEmailAsync(User user, string newEmail, string token);
    }
}