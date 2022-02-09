using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class MessageTemplatePreviewService : IMessageTemplatePreviewService
    {
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly ICurrentSiteLocator _siteLocator;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISession _session;

        public MessageTemplatePreviewService(IMessageTemplateProvider messageTemplateProvider,
            ICurrentSiteLocator siteLocator,
            IServiceProvider serviceProvider, ISession session)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _siteLocator = siteLocator;
            _serviceProvider = serviceProvider;
            _session = session;
        }

        public async Task<MessageTemplate> GetTemplate(string type)
        {
            var site = _siteLocator.GetCurrentSite();
            var messageTemplates = await _messageTemplateProvider.GetAllMessageTemplates(site);
            return messageTemplates.FirstOrDefault(x => x.GetType().FullName == type);
        }

        public async Task<QueuedMessage> GetPreview(string type, int id)
        {
            Type templateType = TypeHelper.GetTypeByName(type);
            var messageTemplateBase = await GetTemplate(type);
            if (messageTemplateBase == null)
                return null;
            var modelType = messageTemplateBase.ModelType;
            if (modelType != null)
            {
                var o = await _session.GetAsync(modelType, id);
                if (o == null)
                    return null;
                var parserType = typeof(IMessageParser<,>).MakeGenericType(templateType, modelType);
                var method = parserType.GetMethod("GetMessage");
                var parser = _serviceProvider.GetRequiredService(parserType);
                var result = await method.InvokeAsync(parser, new object[]
                {
                    o, null, null, null, null, null, null
                });
                return result as QueuedMessage;
            }

            return null;
        }
    }
}