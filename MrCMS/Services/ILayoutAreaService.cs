using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ILayoutAreaService
    {
        LayoutArea GetArea(Layout layout, string name);
        void SaveArea(LayoutArea layoutArea);
        LayoutArea GetArea(int layoutAreaId);
        void DeleteArea(LayoutArea area);
        void SetWidgetOrders(string orders);
        void SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel);
        void ResetSorting(LayoutArea area, Webpage webpage);
    }
}