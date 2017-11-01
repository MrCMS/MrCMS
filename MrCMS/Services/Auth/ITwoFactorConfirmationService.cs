using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface ITwoFactorConfirmationService
    {
        TwoFactorStatus GetStatus();
        Confirm2FAResult TryAndConfirmCode(TwoFactorAuthModel model);
    }
}