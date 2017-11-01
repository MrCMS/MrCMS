using System.Threading.Tasks;
using MrCMS.Entities.People;
using MrCMS.Models.Auth;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class GetVerifiedUserResult : IGetVerifiedUserResult
    {
        private readonly AuthSettings _authSettings;

        public GetVerifiedUserResult(AuthSettings authSettings)
        {
            _authSettings = authSettings;
        }

        public LoginResult GetResult(User user, string returnUrl)
        {
            if (!user.IsActive)
                return new LoginResult { Status = LoginStatus.LockedOut, Message = "User is locked out." };

            var redirectUrl = returnUrl ?? (user.IsAdmin ? "~/admin" : "~");

            if (_authSettings.TwoFactorAuthEnabled)
                return new LoginResult
                {
                    User = user,
                    Status = LoginStatus.TwoFactorRequired,
                    ReturnUrl = redirectUrl
                };

            return new LoginResult
            {
                User = user,
                Status = LoginStatus.Success,
                ReturnUrl = redirectUrl
            };
        }
    }
}