using System;
using System.Web;
using MrCMS.Models.Auth;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class TwoFactorConfirmationService : ITwoFactorConfirmationService
    {
        private readonly HttpSessionStateBase _sessionState;
        private readonly IUserLookup _userLookup;

        public TwoFactorConfirmationService(HttpSessionStateBase sessionState, IUserLookup userLookup)
        {
            _sessionState = sessionState;
            _userLookup = userLookup;
        }

        public TwoFactorStatus GetStatus()
        {
            if (!(_sessionState[SetVerifiedUserData.CurrentUserKey] is Guid guid))
                return TwoFactorStatus.None;

            var user = _userLookup.GetUserByGuid(guid);
            if (user == null)
                return TwoFactorStatus.None;

            return user.TwoFactorCodeExpiry != null && user.TwoFactorCodeExpiry > CurrentRequestData.Now
                ? TwoFactorStatus.Valid
                : TwoFactorStatus.Expired;
        }

        public Confirm2FAResult TryAndConfirmCode(TwoFactorAuthModel model)
        {
            var result = new Confirm2FAResult {ReturnUrl = model.ReturnUrl};
            if (!(_sessionState[SetVerifiedUserData.CurrentUserKey] is Guid guid))
                return result;

            var user = _userLookup.GetUserByGuid(guid);
            if (user == null)
                return result;

            if(user.TwoFactorCodeExpiry == null || user.TwoFactorCodeExpiry < CurrentRequestData.Now)
                return result;

            var code = model.Code?.Trim();
            if (StringComparer.OrdinalIgnoreCase.Equals(user.TwoFactorCode, code))
            {
                result.Success = true;
                result.User = user;
            }
            else
                result.Message = "The code you entered was invalid.";
            return result;
        }
    }
}