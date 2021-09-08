using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using ISession = NHibernate.ISession;

namespace MrCMS.Services.Auth
{
    public class LogSuccessfulLogin : IOnUserLoggedIn
    {
        private readonly SecuritySettings _securitySettings;
        private readonly ISession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogSuccessfulLogin(SecuritySettings securitySettings, ISession session,
            IHttpContextAccessor httpContextAccessor)
        {
            _securitySettings = securitySettings;
            _session = session;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Execute(UserLoggedInEventArgs args)
        {
            if (!_securitySettings.LogLoginAttempts)
                return;
            var request = _httpContextAccessor.HttpContext.Request;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.User?.Email,
                Status = LoginAttemptStatus.Success,
                IpAddress = request.GetCurrentIP(),
                UserAgent = request.UserAgent()
            };
            await _session.TransactAsync(session => session.SaveAsync(loginAttempt));
        }
    }
}