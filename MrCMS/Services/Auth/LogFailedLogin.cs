using System.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services.Auth
{
    public class LogFailedLogin : IOnFailedLogin
    {
        private readonly SecuritySettings _securitySettings;
        private readonly ISession _session;

        public LogFailedLogin(SecuritySettings securitySettings, ISession session)
        {
            _securitySettings = securitySettings;
            _session = session;
        }
        public void Execute(UserFailedLoginEventArgs args)
        {
            if (!_securitySettings.LogLoginAttempts)
                return;
            var request = CurrentRequestData.CurrentContext.Request;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.Email,
                Status = LoginAttemptStatus.Failure,
                IpAddress = request.GetCurrentIP(),
                UserAgent = request.UserAgent
            };
            _session.Transact(session => session.Save(loginAttempt));
        }
    }
}