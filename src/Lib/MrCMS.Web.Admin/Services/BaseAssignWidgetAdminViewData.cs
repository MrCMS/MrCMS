using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using MrCMS.Entities.Widget;

namespace MrCMS.Web.Admin.Services
{
    public abstract class BaseAssignWidgetAdminViewData
    {
        public abstract Task AssignViewDataBase(Widget widget, ViewDataDictionary viewData);
    }

    public abstract class BaseAssignWidgetAdminViewData<T> : BaseAssignWidgetAdminViewData where T : Widget
    {
        public abstract Task AssignViewData(T widget, ViewDataDictionary viewData);

        public sealed override async Task AssignViewDataBase(Widget widget, ViewDataDictionary viewData)
        {
            await AssignViewData(widget as T, viewData);
        }
    }
}