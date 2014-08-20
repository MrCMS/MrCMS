using System;
using MrCMS.Helpers;
using MrCMS.Installation;
using MrCMS.Web.Apps.Core.MessageTemplates;
using NHibernate;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class SetupResetPasswordMessageTemplate :IOnInstallation
    {
        private readonly ISession _session;

        public SetupResetPasswordMessageTemplate(ISession session)
        {
            _session = session;
        }

        public int Priority { get; private set; }
        public void Install(InstallModel model)
        {
            var messageTemplate = new ResetPasswordMessageTemplate
            {
                ToAddress = "{Email}",
                ToName = "{Name}",
                Bcc = String.Empty,
                Cc = String.Empty,
                Subject = String.Format("{0} - Password Reset Request", model.SiteName),
                Body =
                    string.Format("To reset your password please click <a href=\"{0}\">here</a>", "{ResetPasswordUrl}"),
                IsHtml = true
            };
            _session.Transact(session => session.Save(messageTemplate));
        }
    }
}