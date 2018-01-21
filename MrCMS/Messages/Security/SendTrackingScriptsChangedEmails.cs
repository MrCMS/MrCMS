using MrCMS.Events.Documents;
using MrCMS.Events.Settings;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class SendTrackingScriptsChangedEmails : IOnTrackingScriptsChanged
    {
        private readonly SecuritySettings _settings;
        private readonly IMessageParser<TrackingScriptsChangeMessageTemplate, SettingScriptChangeModel> _parser;

        public SendTrackingScriptsChangedEmails(SecuritySettings settings,IMessageParser<TrackingScriptsChangeMessageTemplate, SettingScriptChangeModel> parser)
        {
            _settings = settings;
            _parser = parser;
        }
        public void Execute(ScriptChangedEventArgs<SEOSettings> args)
        {
            if (!_settings.SendScriptChangeNotificationEmails)
                return;
            var message = _parser.GetMessage(new SettingScriptChangeModel
            {
                PreviousValue = args.PreviousValue,
                CurrentValue = args.CurrentValue
            });
            _parser.QueueMessage(message);
        }
    }
}