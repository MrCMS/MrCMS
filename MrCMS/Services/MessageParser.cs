using System.Collections.Generic;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;

namespace MrCMS.Services
{
    public class MessageParser<T> : IMessageParser<T> where T : MessageTemplate, new()
    {
        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly IQueueMessage _queueMessage;
        private readonly Site _site;

        public MessageParser(IQueueMessage queueMessage, IMessageTemplateProvider messageTemplateProvider,
            IMessageTemplateParser messageTemplateParser, Site site)
        {
            _queueMessage = queueMessage;
            _messageTemplateProvider = messageTemplateProvider;
            _messageTemplateParser = messageTemplateParser;
            _site = site;
        }

        public QueuedMessage GetMessage(string fromAddress = null, string fromName = null, string toAddress = null,
            string toName = null, string cc = null, string bcc = null)
        {
            var template = _messageTemplateProvider.GetMessageTemplate<T>(_site);
            if (template == null || template.IsDisabled)
                return null;

            return new QueuedMessage
            {
                FromAddress = _messageTemplateParser.Parse(fromAddress ?? template.FromAddress),
                FromName = _messageTemplateParser.Parse(fromName ?? template.FromName),
                ToAddress = _messageTemplateParser.Parse(toAddress ?? template.ToAddress),
                ToName = _messageTemplateParser.Parse(toName ?? template.ToName),
                Cc = _messageTemplateParser.Parse(cc ?? template.Cc),
                Bcc = _messageTemplateParser.Parse(bcc ?? template.Bcc),
                Subject = _messageTemplateParser.Parse(template.Subject),
                Body = _messageTemplateParser.Parse(template.Body),
                IsHtml = template.IsHtml
            };
        }

        public void QueueMessage(QueuedMessage queuedMessage, List<AttachmentData> attachments = null, bool trySendImmediately = true)
        {
            _queueMessage.Queue(queuedMessage, attachments, trySendImmediately);
        }
    }

    public class MessageParser<T, T2> : IMessageParser<T, T2> where T : MessageTemplate<T2>, new()
    {
        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly IQueueMessage _queueMessage;
        private readonly Site _site;

        public MessageParser(IMessageTemplateParser messageTemplateParser, Site site,
            IMessageTemplateProvider messageTemplateProvider, IQueueMessage queueMessage)
        {
            _messageTemplateParser = messageTemplateParser;
            _site = site;
            _messageTemplateProvider = messageTemplateProvider;
            _queueMessage = queueMessage;
        }

        public QueuedMessage GetMessage(T2 obj, string fromAddress = null, string fromName = null,
            string toAddress = null, string toName = null, string cc = null, string bcc = null)
        {
            var template = _messageTemplateProvider.GetMessageTemplate<T>(_site);
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

        public void QueueMessage(QueuedMessage queuedMessage, List<AttachmentData> attachments = null, bool trySendImmediately = true)
        {
            _queueMessage.Queue(queuedMessage, attachments, trySendImmediately);
        }
    }
}