using System.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services.Auth
{
    public class LogSuccessfulLogin : IOnUserLoggedIn
    {
        private readonly SecuritySettings _securitySettings;
        private readonly ISession _session;

        public LogSuccessfulLogin(SecuritySettings securitySettings, ISession session)
        {
            _securitySettings = securitySettings;
            _session = session;
        }
        public void Execute(UserLoggedInEventArgs args)
        {
            if (!_securitySettings.LogLoginAttempts)
                return;
            var request = CurrentRequestData.CurrentContext.Request;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.User?.Email,
                Status = LoginAttemptStatus.Success,
                IpAddress = request.GetCurrentIP(),
                UserAgent = request.UserAgent
            };
            _session.Transact(session => session.Save(loginAttempt));
        }
    }
}