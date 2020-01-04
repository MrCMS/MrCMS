using System;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Helpers;
using MrCMS.Services;

namespace MrCMS.Settings
{
    public abstract class SiteSettingsBase
    {
        public virtual string TypeName
        {
            get { return GetType().Name.BreakUpString(); }
        }

        public virtual string DivId
        {
            get { return GetType().Name.BreakUpString().ToLowerInvariant().Replace(" ", "-"); }
        }

        public virtual bool RenderInSettings
        {
            get { return false; }
        }

        public virtual void SetViewData(IServiceProvider serviceProvider, ViewDataDictionary viewDataDictionary)
        {
        }
    }
}