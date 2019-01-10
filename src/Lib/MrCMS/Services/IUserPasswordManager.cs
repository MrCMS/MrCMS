using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserPasswordManager
    {
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<bool> HasPasswordAsync(User user);
        Task<IdentityResult> AddPasswordAsync(User user, string password);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<IdentityResult> RemovePasswordAsync(User user);
        Task<IdentityResult> SetPassword(User user, string password);
        Task<string> GetSecurityStampAsync(User user);
        Task<IdentityResult> UpdateSecurityStampAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);
        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);
    }
}