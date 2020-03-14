using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Common;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IWebpageAdminService
    {
        Task<Webpage> GetWebpage(int? id);
        AddWebpageModel GetAddModel(int? id);
        object GetAdditionalPropertyModel(string type);
        Task<Webpage> Add(AddWebpageModel model, object additionalPropertyModel = null);
        Task<IResult<Webpage>> Update(UpdateWebpageViewModel viewModel);
        Task<Webpage> Delete(int id);
        Task<List<SortItem>> GetSortItems(int? parent);
        Task SetOrders(List<SortItem> items);
        Task PublishNow(int id);
        Task Unpublish(int id);
        Task<string> GetServerDate();
    }
}