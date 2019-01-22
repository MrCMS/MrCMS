using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Models.Auth;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class TwoFactorConfirmationService : ITwoFactorConfirmationService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserLookup _userLookup;
        private readonly IGetDateTimeNow _getDateTimeNow;

        public TwoFactorConfirmationService(IHttpContextAccessor contextAccessor, IUserLookup userLookup,IGetDateTimeNow getDateTimeNow)
        {
            _contextAccessor = contextAccessor;
            _userLookup = userLookup;
            _getDateTimeNow = getDateTimeNow;
        }

        public TwoFactorStatus GetStatus()
        {
            var sessionState = _contextAccessor.HttpContext.Session;
            if (!Guid.TryParse(sessionState.GetString(SetVerifiedUserData.CurrentUserKey), out Guid guid))
                return TwoFactorStatus.None;

            var user = _userLookup.GetUserByGuid(guid);
            if (user == null)
                return TwoFactorStatus.None;

            return user.TwoFactorCodeExpiry != null && user.TwoFactorCodeExpiry > _getDateTimeNow.UtcNow
                ? TwoFactorStatus.Valid
                : TwoFactorStatus.Expired;
        }

        public Confirm2FAResult TryAndConfirmCode(TwoFactorAuthModel model)
        {
            var result = new Confirm2FAResult { ReturnUrl = model.ReturnUrl };
            var sessionState = _contextAccessor.HttpContext.Session;
            if (!Guid.TryParse(sessionState.GetString(SetVerifiedUserData.CurrentUserKey), out Guid guid))
                return result;

            var user = _userLookup.GetUserByGuid(guid);
            if (user == null)
                return result;

            if (user.TwoFactorCodeExpiry == null || user.TwoFactorCodeExpiry < _getDateTimeNow.LocalNow)
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