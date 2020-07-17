using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Website;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class LogAdminService : ILogAdminService
    {
        private readonly ISession _session;

        public LogAdminService(ISession session)
        {
            _session = session;
        }


        public void DeleteAllLogs()
        {
            // we need to load these as the hql version doesn't remove from the queries and throws
            var logs = _session.Query<Log>().ToList();
            _session.Transact(session =>
            {
                logs.ForEach(session.Delete);
            });
        }

        public void DeleteLog(int id)
        {
            _session.Transact(session => session.Delete(session.Get<Log>(id)));
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
                {
                    query = query.Where(log => log.Type == searchQuery.Type);
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.Message))
                {
                    query =
                        query.Where(
                            log =>
                                log.Message.IsInsensitiveLike(searchQuery.Message, MatchMode.Anywhere));
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.Detail))
                {
                    query = query.Where(log => log.Detail.IsInsensitiveLike(searchQuery.Detail, MatchMode.Anywhere));
                }

                if (searchQuery.SiteId.HasValue)
                {
                    query = query.Where(log => log.Site.Id == searchQuery.SiteId);
                }

                if (searchQuery.From.HasValue)
                {
                    query = query.Where(log => log.CreatedOn >= searchQuery.From);
                }

                if (searchQuery.To.HasValue)
                {
                    query = query.Where(log => log.CreatedOn <= searchQuery.To);
                }

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