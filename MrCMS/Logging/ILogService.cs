using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;

namespace MrCMS.Logging
{
    public interface ILogService
    {
        IList<LogEntry> GetAllLogEntries();
        IPagedList<LogEntry> GetAllEntriesPaged(int pageNum, int pageSize=10);
        IPagedList<LogEntry> GetAllErrors(int pageNum, int pageSize = 10);
        IPagedList<LogEntry> GetAllAudits(int pageNum, int pageSize = 10);
    }

    public class LogService : ILogService
    {
        private readonly ISession _session;

        public LogService(ISession session)
        {
            _session = session;
        }

        public IList<LogEntry> GetAllLogEntries()
        {
            return BaseQuery().Cacheable().List();
        }

        public IPagedList<LogEntry> GetAllEntriesPaged(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Paged(pageNum, pageSize);
        }

        public IPagedList<LogEntry> GetAllErrors(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Where(entry => entry.Type == LogEntryType.Error).Paged(pageNum, pageSize);
        }

        public IPagedList<LogEntry> GetAllAudits(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Where(entry => entry.Type == LogEntryType.Audit).Paged(pageNum, pageSize);
        }

        private IQueryOver<LogEntry, LogEntry> BaseQuery()
        {
            return
                _session.QueryOver<LogEntry>()
                        .Where(entry => entry.Site == CurrentRequestData.CurrentSite)
                        .OrderBy(entry => entry.CreatedOn)
                        .Desc;
        }
    }
}