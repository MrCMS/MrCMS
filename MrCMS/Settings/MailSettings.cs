using System.Web.Mvc;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Settings
{
    public class MailSettings : ISettings
    {
        public string Host { get; set; }

        public bool UseSSL { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public string GetTypeName()
        {
            return "Mail Settings";
        }

        public string GetDivId()
        {
            return GetTypeName().Replace(" ", "-").ToLower();
        }

        public void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {
            
        }

        public void Save()
        {
            MrCMSApplication.Get<ConfigurationProvider<MailSettings>>().SaveSettings(this);
        }
    }
}