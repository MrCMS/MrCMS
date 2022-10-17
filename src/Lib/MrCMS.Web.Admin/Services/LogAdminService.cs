using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using ISession = NHibernate.ISession;

namespace MrCMS.Web.Admin.Services
{
    public class LogAdminService : ILogAdminService
    {
        private readonly ISession _session;

        public LogAdminService(ISession session)
        {
            _session = session;
        }


        public async Task DeleteAllLogs()
        {
            // we need to load these as the hql version doesn't remove from the queries and throws
            var logs = _session.Query<Log>().ToList();
            await _session.TransactAsync(async session =>
            {
                foreach (var log in logs)
                {
                    await session.DeleteAsync(log);
                }
            });
        }

        public async Task DeleteLog(int id)
        {
            await _session.TransactAsync(async session => await session.DeleteAsync(await session.GetAsync<Log>(id)));
        }

        public async Task<List<SelectListItem>> GetSiteOptions()
        {
            IList<Site> sites = await _session.QueryOver<Site>().OrderBy(site => site.Name).Asc.ListAsync();
            return sites
                .BuildSelectItemList(site => site.Name, site => site.Id.ToString(),
                    emptyItemText: "All sites");
        }

        public async Task<Log> Get(int id)
        {
            return await _session.GetAsync<Log>(id);
        }

        public Task<IPagedList<Log>> GetEntriesPaged(LogSearchQuery searchQuery)
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

            return query.PagedAsync(searchQuery.Page, cacheable: false);
        }

        private IQueryOver<Log, Log> BaseQuery()
        {
            return _session.QueryOver<Log>()
                .OrderBy(entry => entry.Id)
                .Desc;
        }
    }
}