using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Paging;

namespace MrCMS.Logging
{
    public interface ILogService
    {
        void Insert(Log log);
        IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery);
        void DeleteAllLogs();
        void DeleteLog(Log log);
        List<SelectListItem> GetSiteOptions();
    }
}