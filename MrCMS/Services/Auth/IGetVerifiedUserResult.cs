using MrCMS.Entities.People;
using MrCMS.Models.Auth;

namespace MrCMS.Services.Auth
{
    public interface IGetVerifiedUserResult
    {
        LoginResult GetResult(User user, string returnUrl);
    }
}