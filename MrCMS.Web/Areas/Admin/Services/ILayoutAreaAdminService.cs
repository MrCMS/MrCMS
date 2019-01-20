using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ILayoutAreaAdminService
    {
        void Add(LayoutArea layoutArea);
        void Update(LayoutArea layoutArea);
        void DeleteArea(LayoutArea area);
        void SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel);
        void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel);
        void ResetSorting(LayoutArea area, int pageId);
        PageWidgetSortModel GetSortModel(LayoutArea area, int pageId);
    }
}