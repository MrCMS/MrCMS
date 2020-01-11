using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IMediaCategoryAdminService
    {
        AddMediaCategoryModel GetNewCategoryModel(int? id);
        MediaCategory GetCategory(int? id);
        Task<MediaCategory> Add(AddMediaCategoryModel model);
        Task<MediaCategory> Update(UpdateMediaCategoryModel model);
        Task<MediaCategory> Delete(int id);
        List<SortItem> GetSortItems(int id);
        Task SetOrders(List<SortItem> items);
        Task<bool> UrlIsValidForMediaCategory(string urlSegment, int? id);
        UpdateMediaCategoryModel GetEditModel(int id);
    }
}