using MrCMS.Entities.Messaging;
using MrCMS.Entities.People;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Web.Apps.Core.MessageTemplates;

namespace MrCMS.Web.Apps.Core.Services
{
    public class SendResetPasswordEmail : IOnUserResetPasswordSet
    {
        private readonly IMessageParser<ResetPasswordMessageTemplate, User> _messageParser;

        public SendResetPasswordEmail(IMessageParser<ResetPasswordMessageTemplate, User> messageParser)
        {
            _messageParser = messageParser;
        }

        public void Execute(ResetPasswordEventArgs args)
        {
            QueuedMessage queuedMessage = _messageParser.GetMessage(args.User);
            _messageParser.QueueMessage(queuedMessage);
        }
    }
}