using System.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.Auth
{
    public class LogFailedLogin : IOnFailedLogin
    {
        private readonly AuthSettings _authSettings;
        private readonly HttpRequestBase _request;
        private readonly ISession _session;

        public LogFailedLogin(AuthSettings authSettings, HttpRequestBase request, ISession session)
        {
            _authSettings = authSettings;
            _request = request;
            _session = session;
        }
        public void Execute(UserFailedLoginEventArgs args)
        {
            if (!_authSettings.LogLoginAttempts)
                return;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.Email,
                Status = LoginAttemptStatus.Failure,
                IpAddress = _request.GetCurrentIP(),
                UserAgent = _request.UserAgent
            };
            _session.Transact(session => session.Save(loginAttempt));
        }
    }
}