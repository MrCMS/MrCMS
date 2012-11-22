using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using Ninject;

namespace MrCMS.Settings
{
    public class SiteSettings : ISettings
    {
        private Guid _siteId = Guid.NewGuid();
        [ReadOnly(true)]
        public Guid SiteId { get { return _siteId; } set { _siteId = value; } }

        [DisplayName("Default Layout")]
        [DropDownSelection("DefaultLayoutOptions")]
        public int DefaultLayoutId { get; set; }

        [DisplayName("404 Page")]
        [DropDownSelection("404Options")]
        public int Error404PageId { get; set; }
        [DropDownSelection("500Options")]
        [DisplayName("500 Page")]
        public int Error500PageId { get; set; }

        [DisplayName("Media Directory")]
        public string MediaDirectory { get; set; }

        [DisplayName("Site URL")]
        public string SiteUrl { get; set; }

        [DisplayName("System Email Address")]
        public string SystemEmailAddress { get; set; }

        [DisplayName("Site is live")]
        public bool SiteIsLive { get; set; }

        [DisplayName("Site name")]
        public string SiteName { get; set; }

        public string GetTypeName()
        {
            return "Site Settings";
        }

        public string GetDivId()
        {
            return GetTypeName().Replace(" ", "-").ToLower();
        }

        public void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {
            viewDataDictionary["DefaultLayoutOptions"] = GetLayoutOptions(session, DefaultLayoutId);
            viewDataDictionary["404Options"] = GetErrorPageOptions(session, Error404PageId);
            viewDataDictionary["500Options"] = GetErrorPageOptions(session, Error500PageId);
        }

        private List<SelectListItem> GetErrorPageOptions(ISession session, int pageId)
        {
            var queryOver = session.QueryOver<Webpage>().List();
            return
                queryOver.Where(page => page.Published)
                    .BuildSelectItemList(
                        page => page.Name,
                        page => page.Id.ToString(CultureInfo.InvariantCulture),
                        page => page.Id == pageId,
                        "Select page").ToList();
        }

        private List<SelectListItem> GetLayoutOptions(ISession session, int? selectedLayoutId, bool includeDefault = false)
        {
            var selectListItem = SelectListItemHelper.EmptyItem("Default Layout");
            selectListItem.Selected = !selectedLayoutId.HasValue;
            return session.QueryOver<Layout>().Cacheable().List().BuildSelectItemList(
                layout => layout.Name,
                layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                layout => layout.Id == selectedLayoutId,
                includeDefault ? selectListItem : null).ToList();
        }

        public void Save()
        {
            MrCMSApplication.Get<ConfigurationProvider<SiteSettings>>().SaveSettings(this);
        }
    }
}