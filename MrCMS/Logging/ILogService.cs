using System.Collections.Generic;
using MrCMS.Paging;

namespace MrCMS.Logging
{
    public interface ILogService
    {
        IList<Log> GetAllLogEntries();
        IPagedList<Log> GetEntriesPaged(int pageNum, LogEntryType? type, int pageSize = 10);
        void DeleteAllLogs();
        void DeleteLog(Log log);
    }
}