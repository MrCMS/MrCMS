using MrCMS.Entities.People;
using MrCMS.Web.Apps.Core.Models;
using MrCMS.Web.Apps.Core.Models.RegisterAndLogin;

namespace MrCMS.Web.Apps.Core.Services
{
    public interface IResetPasswordService
    {
        void SetResetPassword(User user);
        void ResetPassword(ResetPasswordViewModel model);
    }
}