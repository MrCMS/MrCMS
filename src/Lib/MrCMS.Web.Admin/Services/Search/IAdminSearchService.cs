using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.TextSearch.Services;
using MrCMS.Web.Admin.Models.Search;
using X.PagedList;

namespace MrCMS.Web.Admin.Services.Search
{
    public interface IAdminSearchService
    {
        Task<IPagedList<AdminSearchResult>> Search(ITextSearcher.PagedTextSearcherQuery model);
        List<SelectListItem> GetTypeOptions();
    }
}