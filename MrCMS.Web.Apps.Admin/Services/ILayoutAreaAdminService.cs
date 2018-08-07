using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ILayoutAreaAdminService
    {
        AddLayoutAreaModel GetAddModel(int id);
        void Add(AddLayoutAreaModel layoutArea);
        UpdateLayoutAreaModel GetEditModel(int id);
        Layout GetLayout(int id);
        List<Widget> GetWidgets(int id);
        LayoutArea Update(UpdateLayoutAreaModel model);
        LayoutArea DeleteArea(int id);
        void SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel);
        void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel);
        void ResetSorting(LayoutArea area, int pageId);
        PageWidgetSortModel GetSortModel(LayoutArea area, int pageId);
    }
}