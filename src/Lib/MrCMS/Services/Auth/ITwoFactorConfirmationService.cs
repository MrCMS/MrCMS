using System.Threading.Tasks;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface ITwoFactorConfirmationService
    {
        TwoFactorStatus GetStatus();
        Task<Confirm2FAResult> TryAndConfirmCode(TwoFactorAuthModel model);
    }
}