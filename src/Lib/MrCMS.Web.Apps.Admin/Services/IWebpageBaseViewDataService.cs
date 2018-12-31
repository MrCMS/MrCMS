using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageBaseViewDataService
    {
        void SetAddPageViewData(ViewDataDictionary viewData, Webpage parent);
        void SetEditPageViewData(ViewDataDictionary viewData, Webpage page);
    }
}