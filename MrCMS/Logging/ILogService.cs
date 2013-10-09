using System.Collections.Generic;
using System.ComponentModel;
using MrCMS.Paging;

namespace MrCMS.Logging
{
    public interface ILogService
    {
        IList<Log> GetAllLogEntries();
        IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery);
        void DeleteAllLogs();
        void DeleteLog(Log log);
    }

    public class LogSearchQuery
    {
        public LogSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        [DisplayName("Filter logs")]
        public LogEntryType? Type { get; set; }
    }
}