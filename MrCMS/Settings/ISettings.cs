using System.Web.Mvc;
using NHibernate;

namespace MrCMS.Settings
{
    public interface ISettings
    {
        string GetTypeName();
        string GetDivId();

        void SetViewData(ISession session, ViewDataDictionary viewDataDictionary);

        void Save();
    }
}