using System;
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
        private readonly AuthSettings _settings;
        private readonly IMessageParser<SuccessfulLoginAttemptMessageTemplate, LoginAttempt> _successParser;
        private readonly IMessageParser<FailedLoginAttemptMessageTemplate, LoginAttempt> _failureParser;

        public SendLoginAttemptNotificationEmails(
            AuthSettings settings,
            IMessageParser<SuccessfulLoginAttemptMessageTemplate, LoginAttempt> successParser,
            IMessageParser<FailedLoginAttemptMessageTemplate, LoginAttempt> failureParser
        )
        {
            _settings = settings;
            _successParser = successParser;
            _failureParser = failureParser;
        }
        public void Execute(OnAddedArgs<LoginAttempt> args)
        {
            if (!_settings.SendLoginNotificationEmails)
                return;

            var loginAttempt = args.Item;
            QueuedMessage message;
            switch (loginAttempt.Status)
            {
                case LoginAttemptStatus.Failure:
                    if (loginAttempt.User != null)
                    {
                        message = _failureParser.GetMessage(loginAttempt);
                        _failureParser.QueueMessage(message);
                    }
                    break;
                case LoginAttemptStatus.Success:
                    message = _successParser.GetMessage(loginAttempt);
                    _successParser.QueueMessage(message);
                    break;
            }
        }
    }
}