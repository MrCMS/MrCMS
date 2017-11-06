using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Web.Mvc;
using NHibernate;

namespace MrCMS.Settings
{
    public class SiteSettings : SiteSettingsBase
    {
        private readonly SiteSettingsOptionGenerator _siteSettingsOptionGenerator = new SiteSettingsOptionGenerator();
        private SiteSettingsOptionGenerator _siteSettingsOptionGeneratorOverride;

        public SiteSettings()
        {
            DefaultPageSize = 10;
            Log404s = true;
            CKEditorConfig = SettingDefaults.CkEditorConfig;
            HoneypotFieldName = "YourStatus";
            DaysToKeepLogs = 30;
            TaskExecutorKey = "executor";
            TaskExecutorPassword = Guid.NewGuid().ToString();
            TaskExecutionDelay = 10;
        }

        protected SiteSettingsOptionGenerator SiteSettingsOptionGenerator
        {
            get { return _siteSettingsOptionGeneratorOverride ?? _siteSettingsOptionGenerator; }
        }


        [DropDownSelection("Themes"), DisplayName("Theme")]
        public string ThemeName { get; set; }

        [DisplayName("Default Layout"), DropDownSelection("DefaultLayoutOptions")]
        public int DefaultLayoutId { get; set; }

        [DisplayName("Default Page Size")]
        public int DefaultPageSize { get; set; }

        [DisplayName("Unauthorised Page"), DropDownSelection("403Options")]
        public virtual int Error403PageId { get; set; }

        [DisplayName("404 Page"), DropDownSelection("404Options")]
        public virtual int Error404PageId { get; set; }

        [DropDownSelection("500Options"), DisplayName("500 Page")]
        public virtual int Error500PageId { get; set; }

        [DisplayName("Site is live")]
        public bool SiteIsLive { get; set; }

        [DisplayName("Enable inline editing")]
        public bool EnableInlineEditing { get; set; }

        [DisplayName("Use SSL in Admin")]
        public bool SSLAdmin { get; set; }

        [DisplayName("Use SSL everywhere")]
        public bool SSLEverywhere { get; set; }

        [DisplayName("Log 404 in admin logs")]
        public bool Log404s { get; set; }

        [DisplayName("Raygun API Key")]
        public string RaygunAPIKey { get; set; }
        [DisplayName("Raygun Excluded Status Codes")]
        public string RaygunExcludedStatusCodes { get; set; }

        public IEnumerable<int> RaygunExcludedStatusCodeCollection
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RaygunExcludedStatusCodes)) yield break;
                string[] statusCodes = RaygunExcludedStatusCodes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string statusCode in statusCodes)
                {
                    if (int.TryParse(statusCode, out int id))
                        yield return id;
                }
            }
        }



        [DisplayName("Site UI Culture"), DropDownSelection("UiCultures")]
        public string UICulture { get; set; }

        [MediaSelector]
        public string AdminLogo { get; set; }

        public CultureInfo CultureInfo
        {
            get
            {
                return !String.IsNullOrWhiteSpace(UICulture)
                    ? CultureInfo.GetCultureInfo(UICulture)
                    : CultureInfo.CurrentCulture;
            }
        }

        [DisplayName("Time zones"), DropDownSelection("TimeZones")]
        public string TimeZone { get; set; }

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                return !String.IsNullOrWhiteSpace(TimeZone)
                    ? TimeZoneInfo.FindSystemTimeZoneById(TimeZone)
                    : TimeZoneInfo.Local;
            }
        }

        [DisplayName("Honeypot Field Name")]
        public string HoneypotFieldName { get; set; }

        public string TaskExecutorKey { get; set; }
        public string TaskExecutorPassword { get; set; }
        public int TaskExecutionDelay { get; set; }

        public bool HasHoneyPot
        {
            get { return !string.IsNullOrWhiteSpace(HoneypotFieldName); }
        }

        [DisplayName("CKEditor Config"), TextArea]
        public string CKEditorConfig { get; set; }

        public int DaysToKeepLogs { get; set; }

        [DisplayName("Allowed Admin IPs")]
        public string AllowedAdminIPs { get; set; }

        [DisplayName("MiniProfiler Enabled?")]
        public bool MiniProfilerEnabled { get; set; }


        public IEnumerable<string> AllowedIPs
        {
            get
            {
                if (string.IsNullOrWhiteSpace(AllowedAdminIPs)) yield break;
                string[] ips = AllowedAdminIPs.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string ip in ips)
                {
                    IPAddress address;
                    if (IPAddress.TryParse(ip, out address))
                        yield return ip;
                }
            }
        }

        public override bool RenderInSettings
        {
            get { return true; }
        }

        [DisplayName("Default Form Renderer Type"), DropDownSelection("DefaultFormRenderer")]
        public FormRenderingType FormRendererType { get; set; }


        public void SetSiteSettingsOptionGeneratorOverride(SiteSettingsOptionGenerator siteSettingsOptionGenerator)
        {
            _siteSettingsOptionGeneratorOverride = siteSettingsOptionGenerator;
        }

        public override void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {
            viewDataDictionary["DefaultLayoutOptions"] = SiteSettingsOptionGenerator.GetLayoutOptions(session,
                DefaultLayoutId);
            viewDataDictionary["403Options"] = SiteSettingsOptionGenerator.GetErrorPageOptions(session, Error403PageId);
            viewDataDictionary["404Options"] = SiteSettingsOptionGenerator.GetErrorPageOptions(session, Error404PageId);
            viewDataDictionary["500Options"] = SiteSettingsOptionGenerator.GetErrorPageOptions(session, Error500PageId);
            viewDataDictionary["Themes"] = SiteSettingsOptionGenerator.GetThemeNames(ThemeName);

            viewDataDictionary["UiCultures"] = SiteSettingsOptionGenerator.GetUiCultures(UICulture);

            viewDataDictionary["TimeZones"] = SiteSettingsOptionGenerator.GetTimeZones(TimeZone);

            viewDataDictionary["DefaultFormRenderer"] =
                SiteSettingsOptionGenerator.GetFormRendererOptions(FormRendererType);
        }

        public TagBuilder GetHoneypot()
        {
            var honeyPot = new TagBuilder("input");
            honeyPot.Attributes["type"] = "text";
            honeyPot.Attributes["style"] = "display:none; visibility: hidden;";
            honeyPot.Attributes["name"] = HoneypotFieldName;
            return honeyPot;
        }
    }
}