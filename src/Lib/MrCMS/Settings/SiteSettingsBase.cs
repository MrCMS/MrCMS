using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Helpers;

namespace MrCMS.Settings
{
    public abstract class SiteSettingsBase
    {
        private int _siteId;
        public virtual string TypeName => GetType().Name.BreakUpString();

        public virtual string DivId => GetType().Name.BreakUpString().ToLowerInvariant().Replace(" ", "-");

        public virtual bool RenderInSettings => false;

        public virtual void SetViewData(IServiceProvider serviceProvider, ViewDataDictionary viewDataDictionary)
        {
        }

        public int SiteId
        {
            get => _siteId;
        }

        internal void SetSiteId(int id) => _siteId = id;
    }
}