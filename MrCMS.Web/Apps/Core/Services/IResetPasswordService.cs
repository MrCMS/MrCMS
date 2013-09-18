using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Models;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IResetPasswordService
    {
        void SetResetPassword(User user);
        void ResetPassword(ResetPasswordViewModel model);
    }
}