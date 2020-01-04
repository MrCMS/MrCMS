using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events.Documents;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class SendWebpageFooterScriptsChangedEmails : IOnFooterScriptChanged
    {
        private readonly SecuritySettings _settings;
        private readonly IMessageParser<FooterScriptChangeMessageTemplate, WebpageScriptChangeModel> _parser;
        private readonly IGetLiveUrl _getLiveUrl;

        public SendWebpageFooterScriptsChangedEmails(SecuritySettings settings, IMessageParser<FooterScriptChangeMessageTemplate, WebpageScriptChangeModel> parser, IGetLiveUrl getLiveUrl)
        {
            _settings = settings;
            _parser = parser;
            _getLiveUrl = getLiveUrl;
        }
        public async Task Execute(ScriptChangedEventArgs<Webpage> args)
        {
            if (!_settings.SendScriptChangeNotificationEmails)
                return;
            var message =await _parser.GetMessage(new WebpageScriptChangeModel
            {
                Name = args.Holder?.Name,
                Url = _getLiveUrl.GetAbsoluteUrl(args.Holder),
                PreviousValue = args.PreviousValue,
                CurrentValue = args.CurrentValue
            });
            await _parser.QueueMessage(message);
        }
    }
}