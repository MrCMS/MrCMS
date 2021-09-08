using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Logging;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public interface ILogAdminService
    {
        Task<IPagedList<Log>> GetEntriesPaged(LogSearchQuery searchQuery);
        Task DeleteAllLogs();
        Task DeleteLog(int id);
        Task<List<SelectListItem>> GetSiteOptions();
        Task<Log> Get(int id);
    }
}