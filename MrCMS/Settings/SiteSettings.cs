using System;
using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Settings
{
    public class SiteSettings : SiteSettingsBase
    {
        private Guid _siteId = Guid.NewGuid();
        private readonly SiteSettingsOptionGenerator _siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();

        //[ReadOnly(true)]
        //public Guid SiteId { get { return _siteId; } set { _siteId = value; } }

        [DisplayName("Default Layout")]
        [DropDownSelection("DefaultLayoutOptions")]
        public int DefaultLayoutId { get; set; }

        [DisplayName("404 Page")]
        [DropDownSelection("404Options")]
        public virtual int Error404PageId { get; set; }
        [DropDownSelection("500Options")]
        [DisplayName("500 Page")]
        public virtual int Error500PageId { get; set; }
        
        [DisplayName("Site is live")]
        public bool SiteIsLive { get; set; }
        
        [DisplayName("Enable inline editing")]
        public bool EnableInlineEditing { get; set; }

        public override void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {
            viewDataDictionary["DefaultLayoutOptions"] = _siteSettingsOptionGenerator.GetLayoutOptions(session, Site, DefaultLayoutId);
            viewDataDictionary["404Options"] = _siteSettingsOptionGenerator.GetErrorPageOptions(session, Site, Error404PageId);
            viewDataDictionary["500Options"] = _siteSettingsOptionGenerator.GetErrorPageOptions(session, Site, Error500PageId);
        }
    }
}