using System.Web;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Services.Auth
{
    public class LogLockedOutUserAuthed : IOnLockedOutUserAuthed
    {
        private readonly AuthSettings _authSettings;
        private readonly ISession _session;

        public LogLockedOutUserAuthed(AuthSettings authSettings, ISession session)
        {
            _authSettings = authSettings;
            _session = session;
        }
        public void Execute(UserLockedOutEventArgs args)
        {
            if (!_authSettings.LogLoginAttempts)
                return;
            var request = CurrentRequestData.CurrentContext.Request;
            var loginAttempt = new LoginAttempt
            {
                User = args.User,
                Email = args.User?.Email,
                Status = LoginAttemptStatus.LockedOut,
                IpAddress = request.GetCurrentIP(),
                UserAgent = request.UserAgent
            };
            _session.Transact(session => session.Save(loginAttempt));
        }
    }
}