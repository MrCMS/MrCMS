using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Data;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Messages;
using MrCMS.Services;
using MrCMS.Website;


namespace MrCMS.Web.Apps.Admin.Services
{
    public class MessageTemplatePreviewService : IMessageTemplatePreviewService
    {
        private readonly IMessageTemplateProvider _messageTemplateProvider;
        private readonly IDataReader _dataReader;
        private readonly IGetSiteId _getSiteId;
        private readonly IServiceProvider _serviceProvider;

        public MessageTemplatePreviewService(IMessageTemplateProvider messageTemplateProvider, IDataReader dataReader, IGetSiteId getSiteId, IServiceProvider serviceProvider)
        {
            _messageTemplateProvider = messageTemplateProvider;
            _dataReader = dataReader;
            _getSiteId = getSiteId;
            _serviceProvider = serviceProvider;
        }

        public MessageTemplate GetTemplate(string type)
        {
            return _messageTemplateProvider.GetAllMessageTemplates(_getSiteId.GetId()).FirstOrDefault(x => x.GetType().FullName == type);
        }

        public async Task<QueuedMessage> GetPreview(string type, int id)
        {
            Type templateType = TypeHelper.GetTypeByName(type);
            var messageTemplateBase = GetTemplate(type);
            if (messageTemplateBase == null) 
                return null;
            var modelType = messageTemplateBase.ModelType;
            if (modelType != null)
            {
                var o =await _dataReader.GlobalGet(modelType, id);
                if (o == null)
                    return null;
                var parserType = typeof(IMessageParser<,>).MakeGenericType(templateType, modelType);
                var method = parserType.GetMethod("GetMessage");
                var parser = _serviceProvider.GetRequiredService(parserType);
                return method.Invoke(parser, new object[]
                {
                    o,null,null,null,null,null,null
                }) as QueuedMessage;
            }
            return null;
        }
    }
}