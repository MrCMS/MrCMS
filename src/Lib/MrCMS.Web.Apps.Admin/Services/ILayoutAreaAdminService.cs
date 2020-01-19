using System.Collections.Generic;
using System.Threading.Tasks;
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
        LayoutArea GetArea(int id);
        Layout GetLayout(int id);
        Task<IList<Widget>> GetWidgets(int id);
        LayoutArea Update(UpdateLayoutAreaModel model);
        LayoutArea DeleteArea(int id);
        Task SetWidgetOrders(PageWidgetSortModel pageWidgetSortModel);
        Task SetWidgetForPageOrders(PageWidgetSortModel pageWidgetSortModel);
        Task ResetSorting(int id, int pageId);
        Task<PageWidgetSortModel> GetSortModel(LayoutArea area, int? pageId);
    }
}