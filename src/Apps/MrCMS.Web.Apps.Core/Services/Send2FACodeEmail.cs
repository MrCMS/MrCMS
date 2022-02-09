using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Web.Apps.Core.MessageTemplates;

namespace MrCMS.Web.Apps.Core.Services
{
    public class Send2FACodeEmail : IVerifiedPending2FA
    {
        private readonly IMessageParser<Send2FACodeMessageTemplate, User> _messageParser;

        public Send2FACodeEmail(IMessageParser<Send2FACodeMessageTemplate, User> messageParser)
        {
            _messageParser = messageParser;
        }

        public async Task Execute(VerifiedPending2FAEventArgs args)
        {
            QueuedMessage queuedMessage = await _messageParser.GetMessage(args.User);
            await _messageParser.QueueMessage(queuedMessage);
        }
    }
}