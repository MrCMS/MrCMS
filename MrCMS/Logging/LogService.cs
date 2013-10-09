using System.Collections.Generic;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Logging
{
    public class LogService : ILogService
    {
        private readonly ISession _session;
        private readonly SiteSettings _siteSettings;

        public LogService(ISession session, SiteSettings siteSettings)
        {
            _session = session;
            _siteSettings = siteSettings;
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

        public IPagedList<Log> GetEntriesPaged(int pageNum, LogEntryType? type = null, int pageSize = 10)
        {
            var query = BaseQuery();
            if (type.HasValue)
                query = query.Where(log => log.Type == type);
            return query.Paged(pageNum, pageSize);
        }

        public IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery)
        {
            var query = BaseQuery();
            if (searchQuery.Type.HasValue)
                query = query.Where(log => log.Type == searchQuery.Type);
            return query.Paged(searchQuery.Page, _siteSettings.DefaultPageSize);
        }

        private IQueryOver<Log, Log> BaseQuery()
        {
            return
                _session.QueryOver<Log>()
                        .Where(entry => entry.Site == CurrentRequestData.CurrentSite)
                        .OrderBy(entry => entry.Id)
                        .Desc;
        }
    }
}