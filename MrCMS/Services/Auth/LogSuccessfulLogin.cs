using System.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using NHibernate;

namespace MrCMS.Services.Auth
{
    public class LogSuccessfulLogin : IOnUserLoggedIn
    {
        private readonly AuthSettings _authSettings;
        private readonly HttpRequestBase _request;
        private readonly ISession _session;

        public LogSuccessfulLogin(AuthSettings authSettings, HttpRequestBase request,ISession session)
        {
            _authSettings = authSettings;
            _request = request;
            _session = session;
        }
        public void Execute(UserLoggedInEventArgs args)
        {
            if (!_authSettings.LogLoginAttempts)
                return;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.User?.Email,
                Status = LoginAttemptStatus.Success,
                IpAddress = _request.GetCurrentIP(),
                UserAgent = _request.UserAgent
            };
            _session.Transact(session => session.Save(loginAttempt));
        }
    }
}