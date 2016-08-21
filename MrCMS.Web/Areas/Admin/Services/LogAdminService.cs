using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elmah;
using MrCMS.Data;
using MrCMS.DbConfiguration;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using MrCMS.Paging;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LogAdminService : ILogAdminService
    {
        private readonly IRepository<Log> _logRepository;
        private readonly IRepository<Site> _siteRepository;

        public LogAdminService(IRepository<Log> logRepository,IRepository<Site> siteRepository)
        {
            _logRepository = logRepository;
            _siteRepository = siteRepository;
        }

        public void Insert(Log log)
        {
            if (log.Error == null)
                log.Error = new Error();
            _logRepository.Add(log);
        }


        public void DeleteAllLogs()
        {
            _logRepository.DeleteAll();
        }

        public void DeleteLog(Log log)
        {
            _logRepository.Delete(log);
        }

        public List<SelectListItem> GetSiteOptions()
        {
            IList<Site> sites = _siteRepository.Query().OrderBy(site => site.Name).ToList();
            return sites.Count == 1
                ? new List<SelectListItem>()
                : sites
                    .BuildSelectItemList(site => site.Name, site => site.Id.ToString(),
                        emptyItemText: "All sites");
        }

        public IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery)
        {
            using (_logRepository.DisableSiteFilter())
            {
                var query = BaseQuery();
                if (searchQuery.Type.HasValue)
                    query = query.Where(log => log.Type == searchQuery.Type);

                if (searchQuery.SiteId.HasValue)
                    query = query.Where(log => log.Site.Id == searchQuery.SiteId);

                if (searchQuery.From.HasValue)
                    query = query.Where(log => log.CreatedOn >= searchQuery.From);
                if (searchQuery.To.HasValue)
                    query = query.Where(log => log.CreatedOn <= searchQuery.To);

                if (!string.IsNullOrWhiteSpace(searchQuery.Message))
                    query =
                        query.Where(
                            log =>
                                log.Message.Like($"%{searchQuery.Message}%"));

                if (!string.IsNullOrWhiteSpace(searchQuery.Detail))
                    query = query.Where(log => log.Detail.Like($"%{searchQuery.Detail}%"));

                return query.Paged(searchQuery.Page);
            }
        }

        public IList<Log> GetAllLogEntries()
        {
            return BaseQuery().ToList();
        }

        private IQueryable<Log> BaseQuery()
        {
            return
                _logRepository.Query()
                    .OrderByDescending(log => log.Id);
        }
    }
}