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

        public SendWebpageFooterScriptsChangedEmails(SecuritySettings settings, IMessageParser<FooterScriptChangeMessageTemplate, WebpageScriptChangeModel> parser)
        {
            _settings = settings;
            _parser = parser;
        }
        public void Execute(ScriptChangedEventArgs<Webpage> args)
        {
            if (!_settings.SendScriptChangeNotificationEmails)
                return;
            var message = _parser.GetMessage(new WebpageScriptChangeModel
            {
                Name = args.Holder?.Name,
                Url = args.Holder?.AbsoluteUrl,
                PreviousValue = args.PreviousValue,
                CurrentValue = args.CurrentValue
            });
            _parser.QueueMessage(message);
        }
    }
}