using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IWebpageBaseViewDataService
    {
        void SetAddPageViewData(ViewDataDictionary viewData, Webpage parent);
        void SetEditPageViewData(ViewDataDictionary viewData, Webpage page);
    }
}