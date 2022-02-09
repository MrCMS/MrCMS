using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events.Documents;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class SendWebpageHeaderScriptsChangedEmails : IOnHeaderScriptChanged
    {
        private readonly IMessageParser<HeaderScriptChangeMessageTemplate, WebpageScriptChangeModel> _parser;
        private readonly IGetLiveUrl _getLiveUrl;
        private readonly SecuritySettings _settings;

        public SendWebpageHeaderScriptsChangedEmails(SecuritySettings settings,
            IMessageParser<HeaderScriptChangeMessageTemplate, WebpageScriptChangeModel> parser, IGetLiveUrl getLiveUrl)
        {
            _settings = settings;
            _parser = parser;
            _getLiveUrl = getLiveUrl;
        }

        public async Task Execute(ScriptChangedEventArgs<Webpage> args)
        {
            if (!_settings.SendScriptChangeNotificationEmails)
                return;
            var message = await _parser.GetMessage(new WebpageScriptChangeModel
            {
                Name = args.Holder?.Name,
                Url = await _getLiveUrl.GetAbsoluteUrl(args.Holder),
                PreviousValue = args.PreviousValue,
                CurrentValue = args.CurrentValue
            });
            await _parser.QueueMessage(message);
        }
    }
}