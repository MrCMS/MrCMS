using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public interface IUserPhoneNumberManager
    {
        Task<IdentityResult> SetPhoneNumberAsync(User user, string phoneNumber);
        Task<IdentityResult> ChangePhoneNumberAsync(User user, string phoneNumber, string token);
        Task<bool> IsPhoneNumberConfirmedAsync(User user);
        Task<string> GenerateChangePhoneNumberTokenAsync(User user, string phoneNumber);
        Task<bool> VerifyChangePhoneNumberTokenAsync(User user, string token, string phoneNumber);
    }
}