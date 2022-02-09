using System.Threading.Tasks;
using MrCMS.Events.Documents;
using MrCMS.Events.Settings;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class SendTrackingScriptsBodyChangedEmails : IOnTrackingScriptsBodyChanged
    {
        private readonly SecuritySettings _settings;
        private readonly IMessageParser<TrackingScriptsBodyChangeMessageTemplate, SettingScriptChangeModel> _parser;

        public SendTrackingScriptsBodyChangedEmails(SecuritySettings settings,
            IMessageParser<TrackingScriptsBodyChangeMessageTemplate, SettingScriptChangeModel> parser)
        {
            _settings = settings;
            _parser = parser;
        }

        public async Task Execute(ScriptChangedEventArgs<SEOSettings> args)
        {
            if (!_settings.SendScriptChangeNotificationEmails)
                return;
            var message = await _parser.GetMessage(new SettingScriptChangeModel
            {
                PreviousValue = args.PreviousValue,
                CurrentValue = args.CurrentValue
            });
            await _parser.QueueMessage(message);
        }
    }
}