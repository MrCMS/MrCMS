using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Logging;
using MrCMS.Paging;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface ILogAdminService
    {
        void Insert(Log log);
        IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery);
        void DeleteAllLogs();
        void DeleteLog(Log log);
        List<SelectListItem> GetSiteOptions();
    }
}