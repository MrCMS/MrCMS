using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ISetWidgetAdminViewData
    {
        void SetViewData<T>(ViewDataDictionary viewData, T widget) where T : Widget;
    }
}