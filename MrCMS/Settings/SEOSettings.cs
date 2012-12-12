using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;
using Ninject;

namespace MrCMS.Settings
{
    public class SEOSettings : ISettings
    {
        [DisplayName("Robots.txt file")]
        [TextArea]
        public string RobotsText { get; set; }

        [DisplayName("Google Analytics Account Number")]
        public string GoogleAnalytics { get; set; }

        [DisplayName("Tracking Scripts")]
        [TextArea]
        public string TrackingScripts { get; set; }

        public string GetTypeName()
        {
            return "SEO Settings";
        }

        public string GetDivId()
        {
            return "seo-settings";
        }

        public void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {
            
        }

        public Site Site { get; set; }
    }
}