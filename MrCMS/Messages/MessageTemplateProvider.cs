using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;

namespace MrCMS.Messages
{
    public class MessageTemplateProvider : IMessageTemplateProvider
    {
        private static readonly object SaveLockObject = new object();

        private static readonly Dictionary<Type, Type> DefaultTemplateProviders = new Dictionary<Type, Type>();

        private static readonly MethodInfo GetMessageTemplateMethod = typeof (MessageTemplateProvider)
            .GetMethodExt("GetMessageTemplate", typeof (Site));

        public static readonly MethodInfo GetNewMessageTemplateMethod = typeof (MessageTemplateProvider)
            .GetMethodExt("GetNewMessageTemplate");

        private readonly IKernel _kernel;

        static MessageTemplateProvider()
        {
            foreach (
                Type type in
                    TypeHelper.GetAllConcreteTypesAssignableFrom<MessageTemplateBase>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                HashSet<Type> types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof (GetDefaultTemplate<>).MakeGenericType(type));
                if (types.Any())
                {
                    DefaultTemplateProviders.Add(type, types.First());
                }
            }
        }

        public MessageTemplateProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public void SaveTemplate(MessageTemplateBase messageTemplate)
        {
            lock (SaveLockObject)
            {
                string location = GetFileLocation(messageTemplate);
                messageTemplate.SiteId = null;
                File.WriteAllText(location, Serialize(messageTemplate));
            }
        }

        public void SaveSiteOverride(MessageTemplateBase messageTemplate, Site site)
        {
            string location = GetFileLocation(messageTemplate, site);
            messageTemplate.SiteId = site.Id;
            File.WriteAllText(location, Serialize(messageTemplate));
        }

        public void DeleteSiteOverride(MessageTemplateBase messageTemplate, Site site)
        {
            string fileLocation = GetFileLocation(messageTemplate, site);
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        public List<MessageTemplateBase> GetAllMessageTemplates(Site site)
        {
            HashSet<Type> types = TypeHelper.GetAllConcreteTypesAssignableFrom<MessageTemplateBase>();

            return types.Select(type => GetMessageTemplateMethod.MakeGenericMethod(type)
                .Invoke(this, new[] {site}) as MessageTemplateBase).ToList();
        }

        public T GetMessageTemplate<T>(Site site) where T : MessageTemplateBase, new()
        {
            T template;
            Type templateType = typeof (T);
            string siteFileLocation = GetFileLocation(templateType, site);
            string fileLocation = GetFileLocation(templateType);
            if (File.Exists(siteFileLocation))
            {
                template = JsonConvert.DeserializeObject<T>(File.ReadAllText(siteFileLocation));
            }
            else if (File.Exists(fileLocation))
            {
                template = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileLocation));
            }
            else
            {
                template = GetNewMessageTemplate<T>();
                SaveTemplate(template);
            }
            return template;
        }

        public T GetNewMessageTemplate<T>() where T : MessageTemplateBase, new()
        {
            T template = null;
            Type templateType = typeof (T);
            if (DefaultTemplateProviders.ContainsKey(templateType))
            {
                var getDefaultMessageTemplate =
                    _kernel.Get(DefaultTemplateProviders[templateType]) as IGetDefaultMessageTemplate;
                if (getDefaultMessageTemplate != null)
                    template = getDefaultMessageTemplate.Get() as T;
            }
            return template ?? new T();
        }

        public MessageTemplateBase GetNewMessageTemplate(Type type)
        {
            return GetNewMessageTemplateMethod.MakeGenericMethod(type).Invoke(this, new object[0]) as MessageTemplateBase;
        }

        private string Serialize(MessageTemplateBase template)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new WritablePropertiesOnlyResolver()
            };
            return JsonConvert.SerializeObject(template, settings);
        }

        private string GetFileLocation(MessageTemplateBase messageTemplate, Site site = null)
        {
            return GetFileLocation(messageTemplate.GetType(), site);
        }

        private string GetFileLocation(Type type, Site site = null)
        {
            return string.Format("{0}{1}.json", GetFolder(site), type.FullName.ToLower());
        }

        private string GetFolder(Site site = null)
        {
            string location = string.Format("~/App_Data/Templates/{0}/", site == null ? "system" : site.Id.ToString());
            string mapPath = HostingEnvironment.MapPath(location);
            Directory.CreateDirectory(mapPath);
            return mapPath;
        }

        private class WritablePropertiesOnlyResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.Writable).ToList();
            }
        }
    }
}