using System.Collections.Generic;
using MrCMS.Paging;
using MrCMS.Website;
using NHibernate;
using MrCMS.Helpers;

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

    public class LogService : ILogService
    {
        private readonly ISession _session;

        public LogService(ISession session)
        {
            _session = session;
        }

        public IList<Log> GetAllLogEntries()
        {
            return BaseQuery().Cacheable().List();
        }

        public void DeleteAllLogs()
        {
            _session.CreateQuery("delete Log l").ExecuteUpdate();
        }

        public void DeleteLog(Log log)
        {
            _session.Transact(session => session.Delete(log));
        }

        public IPagedList<Log> GetAllEntriesPaged(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Paged(pageNum, pageSize);
        }

        public IPagedList<Log> GetAllErrors(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Where(entry => entry.Type == LogEntryType.Error).Paged(pageNum, pageSize);
        }

        public IPagedList<Log> GetAllAudits(int pageNum, int pageSize = 10)
        {
            return BaseQuery().Where(entry => entry.Type == LogEntryType.Audit).Paged(pageNum, pageSize);
        }

        private IQueryOver<Log, Log> BaseQuery()
        {
            return
                _session.QueryOver<Log>()
                        .Where(entry => entry.Site == CurrentRequestData.CurrentSite)
                        .OrderBy(entry => entry.CreatedOn)
                        .Desc;
        }
    }
}