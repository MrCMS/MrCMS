using System.Collections.Generic;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface ILayoutAreaService
    {
        LayoutArea GetArea(Layout layout, string name);
        LayoutAreaOverride GetOverride(LayoutArea layoutArea, Webpage webpage);
        LayoutAreaOverride GetOverrideById(int id);
        void SaveArea(LayoutArea layoutArea);
        LayoutArea GetArea(int layoutAreaId);
        void SaveOverride(LayoutAreaOverride layoutAreaOverride);
        void DeleteArea(LayoutArea area);
        void SetWidgetOrders(string orders);
        void SetWidgetForPageOrder(WidgetPageOrder orders);
    }
}