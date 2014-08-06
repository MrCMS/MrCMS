using System.ComponentModel;
using System.Web.Mvc;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Settings
{
    public abstract class SiteSettingsBase
    {
        [ReadOnly(true)]
        public int SiteId { get; set; }

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

        public virtual void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {

        }
    }
}