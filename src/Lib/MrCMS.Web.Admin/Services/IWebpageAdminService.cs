using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Admin.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface IWebpageAdminService
    {
        Task<Webpage> GetWebpage(int? id);
        AddWebpageModel GetAddModel(int? id);
        object GetAdditionalPropertyModel(string type);
        Task<Webpage> Add(AddWebpageModel model, object additionalPropertyModel = null);
        Task<Webpage> Update(UpdateWebpageViewModel viewModel);
        Task<Webpage> Delete(int id);
        Task<List<SortItem>> GetSortItems(int? parent);
        Task SetOrders(List<SortItem> items);
        Task PublishNow(int id);
        Task Unpublish(int id);
        string GetServerDate();
        Task<IPagedList<Select2LookupResult>> Search(string term, int page);
    }
}