using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;
using Ninject;

namespace MrCMS.Settings
{
    public class SEOSettings : SiteSettingsBase
    {
        [DisplayName("Robots.txt file")]
        [TextArea]
        public string RobotsText { get; set; }

        [DisplayName("Tracking Scripts (head)")]
        [TextArea]
        public string TrackingScripts { get; set; }

        [DisplayName("Enable css bundling")]
        public bool EnableCssBundling { get; set; }

        [DisplayName("Enable javascript bundling")]
        public bool EnableJsBundling { get; set; }

        [DisplayName("Enable HTML Minification")]
        public bool EnableHtmlMinification { get; set; }
    }
} 
