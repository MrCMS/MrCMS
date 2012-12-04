using MrCMS.Entities.Documents.Layout;
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
        void SetWidgetForPageOrder(WidgetPageOrder orders);
    }
}