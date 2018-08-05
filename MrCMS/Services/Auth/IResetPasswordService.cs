using MrCMS.Entities.People;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface IResetPasswordService
    {
        void SetResetPassword(User user);
        void ResetPassword(ResetPasswordViewModel model);
    }
}