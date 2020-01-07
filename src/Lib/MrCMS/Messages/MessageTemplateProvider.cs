using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using Newtonsoft.Json;

namespace MrCMS.Messages
{
    public class MessageTemplateProvider : IMessageTemplateProvider
    {
        private static readonly Dictionary<Type, Type> DefaultTemplateProviders = new Dictionary<Type, Type>();

        private static readonly MethodInfo GetMessageTemplateMethod = typeof(MessageTemplateProvider)
            .GetMethodExt("GetMessageTemplate", typeof(int));

        public static readonly MethodInfo GetNewMessageTemplateMethod = typeof(MessageTemplateProvider)
            .GetMethodExt("GetNewMessageTemplate");

        private readonly IGlobalRepository<MessageTemplateData> _repository;
        private readonly IServiceProvider _serviceProvider;

        static MessageTemplateProvider()
        {
            foreach (var type in
                TypeHelper.GetAllConcreteTypesAssignableFrom<MessageTemplate>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(GetDefaultTemplate<>).MakeGenericType(type));
                if (types.Any())
                {
                    DefaultTemplateProviders.Add(type, types.First());
                }
            }
        }

        public MessageTemplateProvider(IGlobalRepository<MessageTemplateData> repository, IServiceProvider serviceProvider)
        {
            _repository = repository;
            _serviceProvider = serviceProvider;
        }

        public async Task SaveTemplate(MessageTemplate messageTemplate)
        {
            await Save(messageTemplate, null);
        }

        public async Task SaveSiteOverride(MessageTemplate messageTemplate, int siteId)
        {
            await Save(messageTemplate, siteId);
        }

        public async Task DeleteSiteOverride(MessageTemplate messageTemplate, int siteId)
        {
            var templateData = await GetExistingTemplateData(messageTemplate.GetType().FullName, siteId);
            if (templateData != null)
                await _repository.Delete(templateData);
        }

        public List<MessageTemplate> GetAllMessageTemplates(int siteId)
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<MessageTemplate>();

            return types.Select(type => GetMessageTemplateMethod.MakeGenericMethod(type)
                .Invoke(this, new object[] { siteId }) as MessageTemplate).ToList();
        }

        public async Task<T> GetMessageTemplate<T>(int siteId) where T : MessageTemplate, new()
        {
            var templateType = typeof(T);
            var fullName = templateType.FullName;
            var templateData = await GetExistingTemplateData(fullName, siteId);
            if (templateData != null)
                return JsonConvert.DeserializeObject<T>(templateData.Data);

            templateData = await GetExistingTemplateData(fullName, null);
            if (templateData != null)
                return JsonConvert.DeserializeObject<T>(templateData.Data);

            var template = GetNewMessageTemplate<T>();
            await SaveTemplate(template);
            return template;
        }

        public MessageTemplate GetNewMessageTemplate(Type type)
        {
            return GetNewMessageTemplateMethod.MakeGenericMethod(type).Invoke(this, new object[0]) as MessageTemplate;
        }

        private async Task Save(MessageTemplate messageTemplate, int? siteId)
        {
            var type = messageTemplate.GetType().FullName;
            var existingData = await GetExistingTemplateData(type, siteId);

            var isNew = existingData == null;
            if (isNew)
            {
                existingData = new MessageTemplateData { Type = type, SiteId = siteId };
            }

            existingData.Data = JsonConvert.SerializeObject(messageTemplate);

            if (isNew)
                await _repository.Add(existingData);
            else
                await _repository.Update(existingData);
        }

        private Task<MessageTemplateData> GetExistingTemplateData(string type, int? siteId)
        {
            return _repository
                .Query()
                .FirstOrDefaultAsync(x => x.Type == type && x.SiteId == siteId);
        }

        public T GetNewMessageTemplate<T>() where T : MessageTemplate, new()
        {
            T template = null;
            var templateType = typeof(T);
            if (DefaultTemplateProviders.ContainsKey(templateType))
            {
                if (_serviceProvider.GetService(DefaultTemplateProviders[templateType]) is IGetDefaultMessageTemplate getDefaultMessageTemplate)
                    template = getDefaultMessageTemplate.Get() as T;
            }
            return template ?? new T();
        }
    }
}