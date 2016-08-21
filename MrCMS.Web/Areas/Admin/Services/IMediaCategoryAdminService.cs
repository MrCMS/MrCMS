using System.Collections.Generic;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IMediaCategoryAdminService
    {
        MediaCategory GetNewCategoryModel(int? id);
        void Add(MediaCategory mediaCategory);
        void Update(MediaCategory mediaCategory);
        void Delete(MediaCategory mediaCategory);
        List<SortItem> GetSortItems(MediaCategory parent);
        void SetOrders(List<SortItem> items);
        bool UrlIsValidForMediaCategory(string urlSegment, int? id);
    }
}