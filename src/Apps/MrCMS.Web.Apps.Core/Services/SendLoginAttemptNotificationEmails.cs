using System;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Web.Apps.Core.MessageTemplates;

namespace MrCMS.Web.Apps.Core.Services
{
    public class SendLoginAttemptNotificationEmails : OnDataAdded<LoginAttempt>
    {
        private readonly SecuritySettings _settings;
        private readonly AuthRoleSettings _roleSettings;
        private readonly IMessageParser<SuccessfulLoginAttemptMessageTemplate, LoginAttempt> _successParser;
        private readonly IMessageParser<FailedLoginAttemptMessageTemplate, LoginAttempt> _failureParser;
        private readonly IJoinTableRepository<UserToRole> _userToRoleRepository;

        public SendLoginAttemptNotificationEmails(
            SecuritySettings settings,
            AuthRoleSettings roleSettings,
            IMessageParser<SuccessfulLoginAttemptMessageTemplate, LoginAttempt> successParser,
            IMessageParser<FailedLoginAttemptMessageTemplate, LoginAttempt> failureParser,
            IJoinTableRepository<UserToRole> userToRoleRepository
        )
        {
            _settings = settings;
            _roleSettings = roleSettings;
            _successParser = successParser;
            _failureParser = failureParser;
            _userToRoleRepository = userToRoleRepository;
        }

        public override async Task Execute(EntityData data)
        {
            if (!_settings.SendLoginNotificationEmails)
                return;

            var loginAttempt = data.Entity() as LoginAttempt;

            if (!_userToRoleRepository.Readonly().Any(x => x.UserId == loginAttempt.UserId && _roleSettings.SendNotificationEmailRoles.Contains(x.RoleId)))
                return;

            QueuedMessage message;
            switch (loginAttempt.Status)
            {
                case LoginAttemptStatus.Failure:
                    message = await _failureParser.GetMessage(loginAttempt);
                    await _failureParser.QueueMessage(message);
                    break;
                case LoginAttemptStatus.Success:
                    message = await _successParser.GetMessage(loginAttempt);
                    await _successParser.QueueMessage(message);
                    break;
            }
        }
    }
}