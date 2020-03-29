using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ILayoutAdminService
    {
        AddLayoutModel GetAddLayoutModel(int? id);
        Layout GetLayout(int? id);
        Task<Layout> Add(AddLayoutModel layout);
        UpdateLayoutModel GetEditModel(int id);
        Task<List<LayoutArea>> GetLayoutAreas(int id);
        Task Update(UpdateLayoutModel layout);
        Task<Layout> Delete(int id);
        Task<List<SortItem>> GetSortItems(int? parent);
        Task SetOrders(List<SortItem> items);
        Task<bool> UrlIsValidForLayout(string urlSegment, int? id);
        Task SetParent(int id, int? parentId);
        IEnumerable<SelectListItem> GetValidParents(int id);
    }

}