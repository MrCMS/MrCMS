using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Elmah;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Paging;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LogAdminService : ILogAdminService
    {
        private readonly ISession _session;

        public LogAdminService(ISession session)
        {
            _session = session;
        }

        public void Insert(Log log)
        {
            if (log.Error == null)
                log.Error = new Error();
            log.Guid = Guid.NewGuid();
            _session.Transact(session => session.Save(log));
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
            IList<Site> sites = _session.QueryOver<Site>().OrderBy(site => site.Name).Asc.List();
            return sites.Count == 1
                ? new List<SelectListItem>()
                : sites
                    .BuildSelectItemList(site => site.Name, site => site.Id.ToString(),
                        emptyItemText: "All sites");
        }

        public IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery)
        {
            using (new SiteFilterDisabler(_session))
            {
                IQueryOver<Log, Log> query = BaseQuery();
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

                return query.Paged(searchQuery.Page);
            }
        }

        public IList<Log> GetAllLogEntries()
        {
            return BaseQuery().Cacheable().List();
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