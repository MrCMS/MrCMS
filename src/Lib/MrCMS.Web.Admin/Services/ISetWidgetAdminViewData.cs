using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Admin.Services
{
    public interface ISetWidgetAdminViewData
    {
        void SetViewData<T>(ViewDataDictionary viewData, T widget) where T : Widget;
        void SetViewDataForAdd(ViewDataDictionary viewData, string type);
    }
}