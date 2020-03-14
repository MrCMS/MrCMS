using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface IResetPasswordService
    {
        Task SetResetPassword(User user);
        Task ResetPassword(ResetPasswordViewModel model);
    }
}