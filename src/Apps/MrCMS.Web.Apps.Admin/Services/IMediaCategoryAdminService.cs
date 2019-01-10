using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMediaCategoryAdminService
    {
        AddMediaCategoryModel GetNewCategoryModel(int? id);
        MediaCategory GetCategory(int? id);
        MediaCategory Add(AddMediaCategoryModel model);
        MediaCategory Update(UpdateMediaCategoryModel model);
        MediaCategory Delete(int id);
        List<SortItem> GetSortItems(int id);
        void SetOrders(List<SortItem> items);
        bool UrlIsValidForMediaCategory(string urlSegment, int? id);
        UpdateMediaCategoryModel GetEditModel(int id);
    }
}