using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Services
{
    public interface IQuickSearcher
    {
        Task<List<QuickSearchResult>> QuickSearch(QuickSearchParams searchParams);
    }
}