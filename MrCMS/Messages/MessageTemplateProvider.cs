using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using Newtonsoft.Json;
using NHibernate;
using Ninject;

namespace MrCMS.Messages
{
    public class MessageTemplateProvider : IMessageTemplateProvider
    {
        private static readonly Dictionary<Type, Type> DefaultTemplateProviders = new Dictionary<Type, Type>();

        private static readonly MethodInfo GetMessageTemplateMethod = typeof (MessageTemplateProvider)
            .GetMethodExt("GetMessageTemplate", typeof (Site));

        public static readonly MethodInfo GetNewMessageTemplateMethod = typeof (MessageTemplateProvider)
            .GetMethodExt("GetNewMessageTemplate");

        private readonly IKernel _kernel;
        private readonly ISession _session;

        static MessageTemplateProvider()
        {
            foreach (var type in
                TypeHelper.GetAllConcreteTypesAssignableFrom<MessageTemplate>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof (GetDefaultTemplate<>).MakeGenericType(type));
                if (types.Any())
                {
                    DefaultTemplateProviders.Add(type, types.First());
                }
            }
        }

        public MessageTemplateProvider(ISession session, IKernel kernel)
        {
            _session = session;
            _kernel = kernel;
        }

        public void SaveTemplate(MessageTemplate messageTemplate)
        {
            Save(messageTemplate, null);
        }

        public void SaveSiteOverride(MessageTemplate messageTemplate, Site site)
        {
            Save(messageTemplate, site.Id);
        }

        public void DeleteSiteOverride(MessageTemplate messageTemplate, Site site)
        {
            var templateData = GetExistingTemplateData(messageTemplate.GetType().FullName, site.Id);
            if (templateData != null)
                _session.Transact(session => session.Delete(templateData));
        }

        public List<MessageTemplate> GetAllMessageTemplates(Site site)
        {
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<MessageTemplate>();

            return types.Select(type => GetMessageTemplateMethod.MakeGenericMethod(type)
                .Invoke(this, new[] {site}) as MessageTemplate).ToList();
        }

        public T GetMessageTemplate<T>(Site site) where T : MessageTemplate, new()
        {
            var templateType = typeof (T);
            var fullName = templateType.FullName;
            var templateData = GetExistingTemplateData(fullName, site.Id);
            if (templateData != null)
                return JsonConvert.DeserializeObject<T>(templateData.Data);

            templateData = GetExistingTemplateData(fullName, null);
            if (templateData != null)
                return JsonConvert.DeserializeObject<T>(templateData.Data);

            var template = GetNewMessageTemplate<T>();
            SaveTemplate(template);
            return template;
        }

        public MessageTemplate GetNewMessageTemplate(Type type)
        {
            return GetNewMessageTemplateMethod.MakeGenericMethod(type).Invoke(this, new object[0]) as MessageTemplate;
        }

        private void Save(MessageTemplate messageTemplate, int? siteId)
        {
            var type = messageTemplate.GetType().FullName;
            var existingData = GetExistingTemplateData(type, siteId) ??
                               new MessageTemplateData {Type = type, SiteId = siteId};

            existingData.Data = JsonConvert.SerializeObject(messageTemplate);

            _session.Transact(session => session.SaveOrUpdate(existingData));
        }

        private MessageTemplateData GetExistingTemplateData(string type, int? siteId)
        {
            return _session.QueryOver<MessageTemplateData>()
                .Where(x => x.Type == type && x.SiteId == siteId)
                .List()
                .FirstOrDefault();
        }

        public T GetNewMessageTemplate<T>() where T : MessageTemplate, new()
        {
            T template = null;
            var templateType = typeof (T);
            if (DefaultTemplateProviders.ContainsKey(templateType))
            {
                var getDefaultMessageTemplate =
                    _kernel.Get(DefaultTemplateProviders[templateType]) as IGetDefaultMessageTemplate;
                if (getDefaultMessageTemplate != null)
                    template = getDefaultMessageTemplate.Get() as T;
            }
            return template ?? new T();
        }
    }
}