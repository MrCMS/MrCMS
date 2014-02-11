using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using MrCMS.Models.Search;
using MrCMS.Paging;

namespace MrCMS.Services.Search
{
    public interface IWebpageSearchService
    {
        IPagedList<Webpage> Search(AdminWebpageSearchQuery model);
        IEnumerable<QuickSearchResults> QuickSearch(AdminWebpageSearchQuery model);
        IEnumerable<Document> GetBreadCrumb(int? parentId);
    }
}
