using System.Threading.Tasks;
using MrCMS.Events.Documents;
using MrCMS.Events.Settings;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class SendTrackingScriptsChangedEmails : IOnTrackingScriptsChanged
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly IMessageParser<TrackingScriptsBodyChangeMessageTemplate, SettingScriptChangeModel> _parser;

        public SendTrackingScriptsChangedEmails(ISystemConfigurationProvider configurationProvider, IMessageParser<TrackingScriptsBodyChangeMessageTemplate, SettingScriptChangeModel> parser)
        {
            _configurationProvider = configurationProvider;
            _parser = parser;
        }

        public async Task Execute(ScriptChangedEventArgs<SEOSettings> args)
        {
            var settings = await _configurationProvider.GetSystemSettings<SecuritySettings>();
            if (!settings.SendScriptChangeNotificationEmails)
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