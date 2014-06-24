using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ILayoutAreaAdminService
    {
        LayoutArea GetArea(Layout layout, string name);
        void SaveArea(LayoutArea layoutArea);
        LayoutArea GetArea(int layoutAreaId);
        void DeleteArea(LayoutArea area);
        void SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel);
        void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel);
        void ResetSorting(LayoutArea area, int pageId);
        PageWidgetSortModel GetSortModel(LayoutArea area, int pageId);
    }
}