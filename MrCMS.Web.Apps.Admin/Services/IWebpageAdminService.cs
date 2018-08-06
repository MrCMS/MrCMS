using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageAdminService
    {
        AddPageModel GetAddModel(int? id);
        void Add(Webpage webpage);
        Webpage Update(UpdateWebpageViewModel viewModel);
        void Delete(Webpage webpage);
        List<SortItem> GetSortItems(Webpage parent);
        void SetOrders(List<SortItem> items);
        void PublishNow(Webpage webpage);
        void Unpublish(Webpage webpage);
    }
}