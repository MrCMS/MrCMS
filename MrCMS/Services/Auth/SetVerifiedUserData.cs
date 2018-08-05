using System;
using System.Web;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using MrCMS.Website;

namespace MrCMS.Services.Auth
{
    public class SetVerifiedUserData : ISetVerifiedUserData
    {
        public const string CurrentUserKey = "2fa-verified-user";
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserManagementService _userManagementService;
        private readonly IGenerate2FACode _generate2FACode;
        private readonly IEventContext _eventContext;

        public SetVerifiedUserData(IHttpContextAccessor contextAccessor, IUserManagementService userManagementService, IGenerate2FACode generate2FACode,
            IEventContext eventContext)
        {
            _contextAccessor = contextAccessor;
            _userManagementService = userManagementService;
            _generate2FACode = generate2FACode;
            _eventContext = eventContext;
        }

        public void SetUserData(User user)
        {
            _contextAccessor.HttpContext.Session.SetString(CurrentUserKey, user.Guid.ToString());
            user.TwoFactorCode = _generate2FACode.GenerateCode();
            user.TwoFactorCodeExpiry = DateTime.UtcNow.AddMinutes(15);
            _userManagementService.SaveUser(user);
            _eventContext.Publish<IVerifiedPending2FA, VerifiedPending2FAEventArgs>(new VerifiedPending2FAEventArgs(user));
        }
    }
}