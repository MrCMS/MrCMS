using MrCMS.Services;
using MrCMS.Web.Apps.Commenting.Entities;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class QueueCommentAddedNotificationEmails : IOnCommentAdded
    {
        private readonly IMessageParser<CommentAddedMessageTemplate, Comment> _messageParser;

        public QueueCommentAddedNotificationEmails(IMessageParser<CommentAddedMessageTemplate, Comment> messageParser)
        {
            _messageParser = messageParser;
        }

        public void CommentAdded(Comment comment)
        {
            var queuedMessage = _messageParser.GetMessage(comment);
            _messageParser.QueueMessage(queuedMessage);
        }
    }
}