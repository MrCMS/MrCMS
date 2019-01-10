using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public abstract class BaseAssignWebpageAdminViewData
    {
        public abstract void AssignViewDataBase(Webpage webpage, ViewDataDictionary viewData);
    }

    public abstract class BaseAssignWebpageAdminViewData<T> : BaseAssignWebpageAdminViewData where T : Webpage
    {
        public abstract void AssignViewData(T webpage, ViewDataDictionary viewData);

        public override sealed void AssignViewDataBase(Webpage webpage, ViewDataDictionary viewData)
        {
            AssignViewData(webpage as T, viewData);
        }
    }
}