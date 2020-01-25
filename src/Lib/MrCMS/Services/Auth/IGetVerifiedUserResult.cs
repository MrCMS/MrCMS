using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface IGetVerifiedUserResult
    {
        Task<LoginResult> GetResult(User user, string returnUrl);
    }
}