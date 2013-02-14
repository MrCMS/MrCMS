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

        [DisplayName("Google Analytics Account Number")]
        public string GoogleAnalytics { get; set; }

        [DisplayName("Tracking Scripts")]
        [TextArea]
        public string TrackingScripts { get; set; }

        [DisplayName("Enable css bundling")]
        public bool EnableCssBundling { get; set; }

        [DisplayName("Enable javascript bundling")]
        public bool EnableJsBundling { get; set; }
    }
}