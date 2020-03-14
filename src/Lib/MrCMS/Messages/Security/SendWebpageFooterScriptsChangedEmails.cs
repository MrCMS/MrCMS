using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Events.Documents;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class SendWebpageFooterScriptsChangedEmails : IOnFooterScriptChanged
    {
        private readonly ISystemConfigurationProvider _configurationProvider;
        private readonly IMessageParser<FooterScriptChangeMessageTemplate, WebpageScriptChangeModel> _parser;
        private readonly IGetLiveUrl _getLiveUrl;

        public SendWebpageFooterScriptsChangedEmails(ISystemConfigurationProvider configurationProvider, IMessageParser<FooterScriptChangeMessageTemplate, WebpageScriptChangeModel> parser, IGetLiveUrl getLiveUrl)
        {
            _configurationProvider = configurationProvider;
            _parser = parser;
            _getLiveUrl = getLiveUrl;
        }
        public async Task Execute(ScriptChangedEventArgs<Webpage> args)
        {
            var settings = await _configurationProvider.GetSystemSettings<SecuritySettings>();
            if (!settings.SendScriptChangeNotificationEmails)
                return;
            var message =await _parser.GetMessage(new WebpageScriptChangeModel
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