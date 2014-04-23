using MrCMS.Services;
using MrCMS.Web.Apps.Commenting.Entities;

namespace MrCMS.Web.Apps.Commenting.Services
{
    public class QueueCommentReportedNotificationEmails : IOnCommentReported
    {
        private readonly IMessageParser<CommentReportedMessageTemplate, Comment> _messageParser;

        public QueueCommentReportedNotificationEmails(IMessageParser<CommentReportedMessageTemplate, Comment> messageParser)
        {
            _messageParser = messageParser;
        }

        public void CommentReported(Comment comment)
        {
            var queuedMessage = _messageParser.GetMessage(comment);
            _messageParser.QueueMessage(queuedMessage);
        }
    }
}