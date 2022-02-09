using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Widget;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ILayoutAreaAdminService
    {
        AddLayoutAreaModel GetAddModel(int id);
        Task Add(AddLayoutAreaModel layoutArea);
        Task<UpdateLayoutAreaModel> GetEditModel(int id);
        Task<LayoutArea> GetArea(int id);
        Task<Layout> GetLayout(int id);
        Task<IList<Widget>> GetWidgets(int id);
        Task<LayoutArea> Update(UpdateLayoutAreaModel model);
        Task<LayoutArea> DeleteArea(int id);
        Task SetWidgetOrders(WidgetSortModel widgetSortModel);
        Task<WidgetSortModel> GetSortModel(LayoutArea area);
    }
}