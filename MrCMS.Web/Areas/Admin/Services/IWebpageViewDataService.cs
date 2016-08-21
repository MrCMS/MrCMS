using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageViewDataService
    {
        void SetAddPageViewData(ViewDataDictionary viewData, Webpage parent);
        void SetEditPageViewData(ViewDataDictionary viewData, Webpage page);
        void SetWebpageAdminViewData(ViewDataDictionary viewData, Webpage webpage);
    }
}