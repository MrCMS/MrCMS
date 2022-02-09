using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface ILayoutAdminService
    {
        AddLayoutModel GetAddLayoutModel(int? id);
        Task<Layout> GetLayout(int? id);
        Task<Layout> Add(AddLayoutModel layout);
        Task<UpdateLayoutModel> GetEditModel(int id);
        Task<List<LayoutArea>> GetLayoutAreas(int id);
        Task Update(UpdateLayoutModel layout);
        Task<Layout> Delete(int id);
        Task<List<SortItem>> GetSortItems(int? parent);
        Task SetOrders(List<SortItem> items);
        Task<bool> UrlIsValidForLayout(string urlSegment, int? id);
        Task SetParent(int id, int? parentId);
        Task<IEnumerable<SelectListItem>> GetValidParents(int id);
    }

}