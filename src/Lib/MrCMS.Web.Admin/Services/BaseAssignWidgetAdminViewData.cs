using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Admin.Services
{
    public abstract class BaseAssignWidgetAdminViewData
    {
        public abstract void AssignViewDataBase(Widget widget, ViewDataDictionary viewData);
    }

    public abstract class BaseAssignWidgetAdminViewData<T> : BaseAssignWidgetAdminViewData where T : Widget
    {
        public abstract void AssignViewData(T widget, ViewDataDictionary viewData);

        public override sealed void AssignViewDataBase(Widget widget, ViewDataDictionary viewData)
        {
            AssignViewData(widget as T, viewData);
        }
    }
}