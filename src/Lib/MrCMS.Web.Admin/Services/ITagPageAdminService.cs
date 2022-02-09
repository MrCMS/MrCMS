using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface ITagPageAdminService
    {
        Task<IList<AutoCompleteResult>> Search(string term);
        Task<IPagedList<Select2LookupResult>> SearchPaged(string term, int page);
        Task<IList<Webpage>> GetWebpages(TagPage page);
        Task<IList<Webpage>> GetWebpages(int pageId);
        Task<Select2LookupResult> GetInfo(int id);
    }
}