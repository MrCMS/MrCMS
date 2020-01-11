using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageAdminService
    {
        Webpage GetWebpage(int? id);
        AddWebpageModel GetAddModel(int? id);
        object GetAdditionalPropertyModel(string type);
        Webpage Add(AddWebpageModel model, object additionalPropertyModel = null);
        Task<Webpage> Update(UpdateWebpageViewModel viewModel);
        Task<Webpage> Delete(int id);
        List<SortItem> GetSortItems(int? parent);
        Task SetOrders(List<SortItem> items);
        Task PublishNow(int id);
        Task Unpublish(int id);
        string GetServerDate();
    }
}