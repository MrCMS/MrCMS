using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ISetWidgetAdminViewData
    {
        void SetViewData<T>(T widget, ViewDataDictionary viewData) where T : Widget;
    }
}