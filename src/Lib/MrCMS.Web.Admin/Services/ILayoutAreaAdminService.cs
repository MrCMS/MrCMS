using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ILayoutAreaAdminService
    {
        AddLayoutAreaModel GetAddModel(int id);
        void Add(AddLayoutAreaModel layoutArea);
        UpdateLayoutAreaModel GetEditModel(int id);
        LayoutArea GetArea(int id);
        Layout GetLayout(int id);
        IList<Widget> GetWidgets(int id);
        LayoutArea Update(UpdateLayoutAreaModel model);
        LayoutArea DeleteArea(int id);
        void SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel);
        void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel);
        void ResetSorting(int id, int pageId);
        PageWidgetSortModel GetSortModel(LayoutArea area, int pageId);
    }
}