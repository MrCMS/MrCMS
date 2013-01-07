using System.Web.Mvc;
using MrCMS.Entities.Multisite;
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
    }
    public abstract class GlobalSettingsBase : SettingsBase
    {
    }
    public abstract class SiteSettingsBase : SettingsBase
    {
        public Site Site { get; set; }
    }

    //public interface ISettings
    //{
    //    string GetTypeName();
    //    string GetDivId();

    //    void SetViewData(ISession session, ViewDataDictionary viewDataDictionary);
    //}

    //public interface ISiteSettings
    //{
    //    string GetTypeName();
    //    string GetDivId();

    //    void SetViewData(ISession session, ViewDataDictionary viewDataDictionary);
    //    Site Site { get; set; }
    //}
}