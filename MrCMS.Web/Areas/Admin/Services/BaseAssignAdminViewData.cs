using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public abstract class BaseAssignAdminViewData
    {
        public abstract void AssignViewData(Webpage webpage, ViewDataDictionary viewData);
    }

    public abstract class BaseAssignAdminViewData<T> : BaseAssignAdminViewData where T : Webpage
    {
        public abstract void AssignViewData(T webpage, ViewDataDictionary viewData);

        public override sealed void AssignViewData(Webpage webpage, ViewDataDictionary viewData)
        {
            AssignViewData(webpage as T, viewData);
        }
    }
}