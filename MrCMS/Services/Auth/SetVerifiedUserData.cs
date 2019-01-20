using System.Web;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class SetVerifiedUserData : ISetVerifiedUserData
    {
        public const string CurrentUserKey = "2fa-verified-user";
        private readonly HttpSessionStateBase _sessionState;
        private readonly IUserManagementService _userManagementService;
        private readonly IGenerate2FACode _generate2FACode;

        public SetVerifiedUserData(HttpSessionStateBase sessionState, IUserManagementService userManagementService, IGenerate2FACode generate2FACode)
        {
            _sessionState = sessionState;
            _userManagementService = userManagementService;
            _generate2FACode = generate2FACode;
        }

        public void SetUserData(User user)
        {
            _sessionState[CurrentUserKey] = user.Guid;
            user.TwoFactorCode = _generate2FACode.GenerateCode();
            user.TwoFactorCodeExpiry = CurrentRequestData.Now.AddMinutes(15);
            _userManagementService.SaveUser(user);
            EventContext.Instance.Publish<IVerifiedPending2FA, VerifiedPending2FAEventArgs>(new VerifiedPending2FAEventArgs(user));
        }
    }
}