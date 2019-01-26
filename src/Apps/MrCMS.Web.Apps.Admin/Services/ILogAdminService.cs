using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Logging;
using X.PagedList;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface ILogAdminService
    {
        IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery);
        void DeleteAllLogs();
        void DeleteLog(int id);
        List<SelectListItem> GetSiteOptions();
    }
}