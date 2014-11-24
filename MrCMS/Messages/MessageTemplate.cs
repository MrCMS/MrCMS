using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Helpers;
using System.Web.Hosting;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using Newtonsoft.Json;

namespace MrCMS.Messages
{
    public interface IMessageTemplateProvider
    {
        void SaveTemplate(MessageTemplateBase messageTemplate);
        void SaveSiteOverride(MessageTemplateBase messageTemplate, Site site);
        void DeleteSiteOverride(MessageTemplateBase messageTemplate, Site site);
        List<MessageTemplateBase> GetAllMessageTemplates(Site site);
        T GetMessageTemplate<T>(Site site) where T : MessageTemplateBase, new();
    }

    public abstract class MessageTemplateBase<T> : MessageTemplateBase
    {
        public override sealed Type ModelType { get { return typeof(T); } }
    }

    public abstract class MessageTemplateBase
    {
        [Required]
        [Display(Name = "From Address")]
        public string FromAddress { get; set; }

        [Required]
        [Display(Name = "From Name")]
        public string FromName { get; set; }

        [Required]
        [Display(Name = "To Address")]
        public string ToAddress { get; set; }

        [Display(Name = "To Name")]
        public string ToName { get; set; }

        public string Cc { get; set; }
        public string Bcc { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        [Display(Name = "Is HTML?")]
        public bool IsHtml { get; set; }

        public virtual Type ModelType { get { return null; } }

        public int? SiteId { get; set; }
    }

    public class MessageTemplateProvider : IMessageTemplateProvider
    {

        private static readonly object SaveLockObject = new object();
        private static readonly object ReadLockObject = new object();

        private static readonly MethodInfo GetMessageTemplateMethod = typeof (MessageTemplateProvider)
            .GetMethodExt("GetMessageTemplate",typeof (Site));

        public void SaveTemplate(MessageTemplateBase messageTemplate)
        {
            lock (SaveLockObject)
            {
                string location = GetFileLocation(messageTemplate);
                messageTemplate.SiteId = null;
                File.WriteAllText(location, JsonConvert.SerializeObject(messageTemplate));
            }
        }

        public void SaveSiteOverride(MessageTemplateBase messageTemplate, Site site)
        {
            string location = GetFileLocation(messageTemplate, site);
            messageTemplate.SiteId = site.Id;
            File.WriteAllText(location, JsonConvert.SerializeObject(messageTemplate));
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
            var types = TypeHelper.GetAllConcreteTypesAssignableFrom<MessageTemplateBase>();

            return types.Select(type => GetMessageTemplateMethod.MakeGenericMethod(type)
                .Invoke(this, new[] { site }) as MessageTemplateBase).ToList();
        }

        public T GetMessageTemplate<T>(Site site) where T : MessageTemplateBase, new()
        {
            T template;
            var siteFileLocation = GetFileLocation(typeof(T), site);
            var fileLocation = GetFileLocation(typeof(T));
            if (File.Exists(siteFileLocation))
                template = JsonConvert.DeserializeObject<T>(File.ReadAllText(siteFileLocation));

            else if (File.Exists(fileLocation))
            {
                template= JsonConvert.DeserializeObject<T>(File.ReadAllText(fileLocation));
            }
            else
            {
                template = new T();
                SaveTemplate(template);
            }
            return template;
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
    }
}