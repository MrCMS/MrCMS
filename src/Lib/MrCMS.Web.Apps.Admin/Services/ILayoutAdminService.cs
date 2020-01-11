using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ILayoutAdminService
    {
        AddLayoutModel GetAddLayoutModel(int? id);
        Layout GetLayout(int? id);
        Layout Add(AddLayoutModel layout);
        UpdateLayoutModel GetEditModel(int id);
        List<LayoutArea> GetLayoutAreas(int id);
        void Update(UpdateLayoutModel layout);
        Layout Delete(int id);
        List<SortItem> GetSortItems(int? parent);
        Task SetOrders(List<SortItem> items);
        Task<bool> UrlIsValidForLayout(string urlSegment, int? id);
        Task SetParent(int id, int? parentId);
        IEnumerable<SelectListItem> GetValidParents(int id);
    }

}