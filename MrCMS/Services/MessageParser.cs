using MrCMS.Entities.Messaging;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Services
{
    public class MessageParser<T, T2> : IMessageParser<T, T2> where T : MessageTemplate, IMessageTemplate<T2>
    {
        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly ISession _session;

        public MessageParser(IMessageTemplateParser messageTemplateParser, ISession session)
        {
            _messageTemplateParser = messageTemplateParser;
            _session = session;
        }

        public QueuedMessage GetMessage(T2 obj, string fromAddress = null, string fromName = null, string toAddress = null, string toName = null, string cc = null, string bcc = null)
        {
            var template = _session.QueryOver<T>().Cacheable().SingleOrDefault();
            if (template == null)
                return null;

            return new QueuedMessage
                {
                    FromAddress = _messageTemplateParser.Parse(fromAddress ?? template.FromAddress, obj),
                    FromName = _messageTemplateParser.Parse(fromName ?? template.FromName, obj),
                    ToAddress = _messageTemplateParser.Parse(toAddress ?? template.ToAddress, obj),
                    ToName = _messageTemplateParser.Parse(toName ?? template.ToName, obj),
                    Cc = _messageTemplateParser.Parse(cc ?? template.Cc, obj),
                    Bcc = _messageTemplateParser.Parse(bcc ?? template.Bcc, obj),
                    Subject = _messageTemplateParser.Parse(template.Subject, obj),
                    Body = _messageTemplateParser.Parse(template.Body, obj),
                    IsHtml = template.IsHtml
                };
        }

        public void QueueMessage(QueuedMessage queuedMessage)
        {
            if (queuedMessage != null)
                _session.Transact(session => session.Save(queuedMessage));
        }
    }
}