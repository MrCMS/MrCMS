using System.Web.Mvc;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Settings
{
    public abstract class SettingsBase
    {
        public virtual string TypeName
        {
            get { return GetType().Name.BreakUpString(); }
        }

        public virtual string DivId
        {
            get { return GetType().Name.BreakUpString().ToLowerInvariant().Replace(" ", "-"); }
        }

        public virtual void SetViewData(ISession session, ViewDataDictionary viewDataDictionary)
        {

        }

        public virtual bool RenderInSettings
        {
            get { return true; }
        }
    }
}