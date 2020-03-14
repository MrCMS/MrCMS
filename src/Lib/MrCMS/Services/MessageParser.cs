using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Messages;
using MrCMS.Website;

namespace MrCMS.Services
{
    public class MessageParser<T> : IMessageParser<T> where T : MessageTemplate, new()
    {
        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly IGetSiteId _getSiteId;
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly IQueueMessage _queueMessage;

        public MessageParser(IQueueMessage queueMessage, IMessageTemplateProvider messageTemplateProvider,
            IMessageTemplateParser messageTemplateParser, IGetSiteId getSiteId)
        {
            _queueMessage = queueMessage;
            _messageTemplateProvider = messageTemplateProvider;
            _messageTemplateParser = messageTemplateParser;
            _getSiteId = getSiteId;
        }

        public async Task<QueuedMessage> GetMessage(string fromAddress = null, string fromName = null,
            string toAddress = null,
            string toName = null, string cc = null, string bcc = null)
        {
            var template = await _messageTemplateProvider.GetMessageTemplate<T>(_getSiteId.GetId());
            if (template == null || template.IsDisabled)
                return null;

            return new QueuedMessage
            {
                FromAddress = await _messageTemplateParser.Parse(fromAddress ?? template.FromAddress),
                FromName = await _messageTemplateParser.Parse(fromName ?? template.FromName),
                ToAddress = await _messageTemplateParser.Parse(toAddress ?? template.ToAddress),
                ToName = await _messageTemplateParser.Parse(toName ?? template.ToName),
                Cc = await _messageTemplateParser.Parse(cc ?? template.Cc),
                Bcc = await _messageTemplateParser.Parse(bcc ?? template.Bcc),
                Subject = await _messageTemplateParser.Parse(template.Subject),
                Body = await _messageTemplateParser.Parse(template.Body),
                IsHtml = template.IsHtml
            };
        }

        public async Task QueueMessage(QueuedMessage queuedMessage, List<AttachmentData> attachments = null,
            bool trySendImmediately = true)
        {
            await _queueMessage.Queue(queuedMessage, attachments, trySendImmediately);
        }
    }

    public class MessageParser<T, T2> : IMessageParser<T, T2> where T : MessageTemplate<T2>, new()
    {
        private readonly IMessageTemplateParser _messageTemplateParser;
        private readonly IGetSiteId _getSiteId;
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly IQueueMessage _queueMessage;

        public MessageParser(IMessageTemplateParser messageTemplateParser, IGetSiteId getSiteId,
            IMessageTemplateProvider messageTemplateProvider, IQueueMessage queueMessage)
        {
            _messageTemplateParser = messageTemplateParser;
            _getSiteId = getSiteId;
            _messageTemplateProvider = messageTemplateProvider;
            _queueMessage = queueMessage;
        }

        public async Task<QueuedMessage> GetMessage(T2 obj, string fromAddress = null, string fromName = null,
            string toAddress = null, string toName = null, string cc = null, string bcc = null)
        {
            var template = await _messageTemplateProvider.GetMessageTemplate<T>(_getSiteId.GetId());
            if (template == null)
                return null;

            return new QueuedMessage
            {
                FromAddress = await _messageTemplateParser.Parse(fromAddress ?? template.FromAddress, obj),
                FromName = await _messageTemplateParser.Parse(fromName ?? template.FromName, obj),
                ToAddress = await _messageTemplateParser.Parse(toAddress ?? template.ToAddress, obj),
                ToName = await _messageTemplateParser.Parse(toName ?? template.ToName, obj),
                Cc = await _messageTemplateParser.Parse(cc ?? template.Cc, obj),
                Bcc = await _messageTemplateParser.Parse(bcc ?? template.Bcc, obj),
                Subject = await _messageTemplateParser.Parse(template.Subject, obj),
                Body = await _messageTemplateParser.Parse(template.Body, obj),
                IsHtml = template.IsHtml
            };
        }

        public async Task QueueMessage(QueuedMessage queuedMessage, List<AttachmentData> attachments = null,
            bool trySendImmediately = true)
        {
            await _queueMessage.Queue(queuedMessage, attachments, trySendImmediately);
        }
    }
}