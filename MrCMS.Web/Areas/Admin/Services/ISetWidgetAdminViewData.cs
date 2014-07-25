using System.Web.Mvc;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ISetWidgetAdminViewData
    {
        void SetViewData<T>(T widget, ViewDataDictionary viewData) where T : Widget;
    }
}