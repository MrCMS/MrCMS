using System.Collections.Generic;
using MrCMS.Paging;

namespace MrCMS.Logging
{
    public interface ILogService
    {
        IList<Log> GetAllLogEntries();
        IPagedList<Log> GetAllEntriesPaged(int pageNum, int pageSize=10);
        IPagedList<Log> GetAllErrors(int pageNum, int pageSize = 10);
        IPagedList<Log> GetAllAudits(int pageNum, int pageSize = 10);
        void DeleteAllLogs();
        void DeleteLog(Log log);
    }
}