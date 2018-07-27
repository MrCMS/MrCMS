using MrCMS.Entities.Documents.Web;
using MrCMS.Events.Documents;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Messages.Security
{
    public class SendWebpageHeaderScriptsChangedEmails : IOnHeaderScriptChanged
    {
        private readonly IMessageParser<HeaderScriptChangeMessageTemplate, WebpageScriptChangeModel> _parser;
        private readonly SecuritySettings _settings;

        public SendWebpageHeaderScriptsChangedEmails(SecuritySettings settings, IMessageParser<HeaderScriptChangeMessageTemplate, WebpageScriptChangeModel> parser)
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