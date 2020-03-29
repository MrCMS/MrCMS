using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Logging;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ILogAdminService
    {
        IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery);
        Task DeleteAllLogs();
        Task DeleteLog(int id);
        List<SelectListItem> GetSiteOptions();
    }
}