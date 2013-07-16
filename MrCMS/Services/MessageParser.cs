using MrCMS.Entities.Messaging;
using NHibernate;

namespace MrCMS.Services
{
    public class MessageParser<T, T2> : IMessageParser<T, T2> where T : MessageTemplate, IMessageTemplate<T2>
    {
        private readonly INotificationTemplateProcessor _notificationTemplateProcessor;
        private readonly ISession _session;

        public MessageParser(INotificationTemplateProcessor notificationTemplateProcessor, ISession session)
        {
            _notificationTemplateProcessor = notificationTemplateProcessor;
            _session = session;
        }

        public QueuedMessage GetMessage(T2 obj, string fromAddress = null, string fromName = null, string toAddress = null, string toName = null, string cc = null, string bcc = null)
        {
            var template = _session.QueryOver<T>().Cacheable().SingleOrDefault();

            return new QueuedMessage
                {
                    FromAddress = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, fromAddress ?? template.FromAddress),
                    FromName = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, fromName ?? template.FromName),
                    ToAddress = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, toAddress ?? template.ToAddress),
                    ToName = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, toName ?? template.ToName),
                    Cc = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, cc ?? template.Cc),
                    Bcc = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, bcc ?? template.Bcc),
                    Subject = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, template.Subject),
                    Body = _notificationTemplateProcessor.ReplaceTokensAndMethods(obj, template.Body),
                    IsHtml = template.IsHtml
                };
        }
    }
}