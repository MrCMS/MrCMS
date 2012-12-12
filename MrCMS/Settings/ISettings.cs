using System.Web.Mvc;
using MrCMS.Entities.Multisite;
using NHibernate;

namespace MrCMS.Settings
{
    public interface ISettings
    {
        string GetTypeName();
        string GetDivId();

        void SetViewData(ISession session, ViewDataDictionary viewDataDictionary);

        Site Site { get; set; }
    }
}