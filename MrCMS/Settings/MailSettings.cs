using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;
using Ninject;

namespace MrCMS.Settings
{
    public class MailSettings : SiteSettingsBase
    {
        [DisplayName("System Email Address")]
        public string SystemEmailAddress { get; set; }

        public string Host { get; set; }

        public bool UseSSL { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }
    }
}