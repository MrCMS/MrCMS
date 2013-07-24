using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;
using NHibernate;

namespace MrCMS.Settings
{
    public class SiteSettings : SiteSettingsBase
    {
        private readonly SiteSettingsOptionGenerator _siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();
        private SiteSettingsOptionGenerator _siteSettingsOptionGeneratorOverride;
        protected SiteSettingsOptionGenerator SiteSettingsOptionGenerator { get { return _siteSettingsOptionGeneratorOverride ?? _siteSettingsOptionGenerator; } }
        public void SetSiteSettingsOptionGeneratorOverride(SiteSettingsOptionGenerator siteSettingsOptionGenerator)
        {
            _siteSettingsOptionGeneratorOverride = siteSettingsOptionGenerator;
        }

        [DropDownSelection("Themes")]
        [DisplayName("Theme")]
        public string ThemeName { get; set; }

        [DisplayName("Default Layout")]
        [DropDownSelection("DefaultLayoutOptions")]
        public int DefaultLayoutId { get; set; }

        [DisplayName("Unauthorised Page")]
        [DropDownSelection("403Options")]
        public virtual int Error403PageId { get; set; }
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

        [DisplayName("Use SSL in Admin")]
        public bool SSLAdmin { get; set; }

        [DisplayName("Site UI Culture")]
        [DropDownSelection("UiCultures")]
        public string UICulture { get; set; }
        public CultureInfo CultureInfo { get { return !string.IsNullOrWhiteSpace(UICulture) ? CultureInfo.GetCultureInfo(UICulture) : CultureInfo.CurrentCulture; } }

        [DisplayName("Time zones")]
        [DropDownSelection("TimeZones")]
        public string TimeZone { get; set; }
        public TimeZoneInfo TimeZoneInfo { get { return !string.IsNullOrWhiteSpace(TimeZone) ? TimeZoneInfo.FindSystemTimeZoneById(TimeZone) : TimeZoneInfo.Local; } }

        public override void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {
            viewDataDictionary["DefaultLayoutOptions"] = SiteSettingsOptionGenerator.GetLayoutOptions(session, Site,
                                                                                                       DefaultLayoutId);
            viewDataDictionary["403Options"] = SiteSettingsOptionGenerator.GetErrorPageOptions(session, Site,
                                                                                                Error403PageId);
            viewDataDictionary["404Options"] = SiteSettingsOptionGenerator.GetErrorPageOptions(session, Site,
                                                                                                Error404PageId);
            viewDataDictionary["500Options"] = SiteSettingsOptionGenerator.GetErrorPageOptions(session, Site,
                                                                                                Error500PageId);
            viewDataDictionary["Themes"] = SiteSettingsOptionGenerator.GetThemeNames(ThemeName);

            viewDataDictionary["UiCultures"] = SiteSettingsOptionGenerator.GetUiCultures(UICulture);

            viewDataDictionary["TimeZones"] = SiteSettingsOptionGenerator.GetTimeZones(TimeZone);
        }

    }
}