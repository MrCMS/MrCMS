using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Elmah;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Paging;
using MrCMS.Settings;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;

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

        public void Insert(Log log)
        {
            if (log.Error == null)
                log.Error = new Error();
            log.Guid = Guid.NewGuid();
            _session.Transact(session => session.Save(log));
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

        public List<SelectListItem> GetSiteOptions()
        {
            var sites = _session.QueryOver<Site>().OrderBy(site => site.Name).Asc.List();
            return sites.Count == 1
                       ? new List<SelectListItem>()
                       : sites
                             .BuildSelectItemList(site => site.Name, site => site.Id.ToString(),
                                                  emptyItemText: "All sites");
        }

        public IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery)
        {
            var query = BaseQuery();
            if (searchQuery.Type.HasValue)
                query = query.Where(log => log.Type == searchQuery.Type);

            if (!string.IsNullOrWhiteSpace(searchQuery.Message))
                query =
                    query.Where(
                        log =>
                        log.Message.IsInsensitiveLike(searchQuery.Message, MatchMode.Anywhere));

            if (!string.IsNullOrWhiteSpace(searchQuery.Detail))
                query = query.Where(log => log.Detail.IsInsensitiveLike(searchQuery.Detail, MatchMode.Anywhere));

            if (searchQuery.SiteId.HasValue)
                query = query.Where(log => log.Site.Id == searchQuery.SiteId);
            
            if (searchQuery.From.HasValue)
                query = query.Where(log => log.CreatedOn >= searchQuery.From);
            if (searchQuery.To.HasValue)
                query = query.Where(log => log.CreatedOn <= searchQuery.To);

            return query.Paged(searchQuery.Page, _siteSettings.DefaultPageSize);
        }

        private IQueryOver<Log, Log> BaseQuery()
        {
            return
                _session.QueryOver<Log>()
                        .OrderBy(entry => entry.Id)
                        .Desc;
        }
    }
}