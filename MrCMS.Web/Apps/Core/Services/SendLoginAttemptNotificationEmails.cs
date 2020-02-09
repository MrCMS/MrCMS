using System;
using System.Linq;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.MessageTemplates;

namespace MrCMS.Web.Apps.Core.Services
{
    public class SendLoginAttemptNotificationEmails : IOnAdded<LoginAttempt>
    {
        private readonly SecuritySettings _settings;
        private readonly AuthRoleSettings _roleSettings;
        private readonly IMessageParser<SuccessfulLoginAttemptMessageTemplate, LoginAttempt> _successParser;
        private readonly IMessageParser<FailedLoginAttemptMessageTemplate, LoginAttempt> _failureParser;

        public SendLoginAttemptNotificationEmails(
            SecuritySettings settings,
            AuthRoleSettings roleSettings,
            IMessageParser<SuccessfulLoginAttemptMessageTemplate, LoginAttempt> successParser,
            IMessageParser<FailedLoginAttemptMessageTemplate, LoginAttempt> failureParser
        )
        {
            _settings = settings;
            _roleSettings = roleSettings;
            _successParser = successParser;
            _failureParser = failureParser;
        }
        public void Execute(OnAddedArgs<LoginAttempt> args)
        {
            if (!_settings.SendLoginNotificationEmails)
                return;

            var loginAttempt = args.Item;
            var user = loginAttempt?.User;
            if (user == null)
                return;

            if (!user.Roles.Any(role => _roleSettings.SendNotificationEmailRoles.Contains(role.Id)))
                return;

            QueuedMessage message;
            switch (loginAttempt.Status)
            {
                case LoginAttemptStatus.Failure:
                    message = _failureParser.GetMessage(loginAttempt);
                    _failureParser.QueueMessage(message);
                    break;
                case LoginAttemptStatus.Success:
                    message = _successParser.GetMessage(loginAttempt);
                    _successParser.QueueMessage(message);
                    break;
            }
        }
    }
}