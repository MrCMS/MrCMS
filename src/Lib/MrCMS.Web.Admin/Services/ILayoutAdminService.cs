using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
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
        void SetOrders(List<SortItem> items);
        bool UrlIsValidForLayout(string urlSegment, int? id);
        void SetParent(int id, int? parentId);
        IEnumerable<SelectListItem> GetValidParents(int id);
    }

}