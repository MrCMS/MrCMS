using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Logging;
using X.PagedList;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class LogAdminService : ILogAdminService
    {
        private readonly IGlobalRepository<Log> _repository;
        private readonly IGlobalRepository<Site> _siteRepository;

        public LogAdminService(IGlobalRepository<Log> repository, IGlobalRepository<Site> siteRepository)
        {
            _repository = repository;
            _siteRepository = siteRepository;
        }


        public async Task DeleteAllLogs()
        {
            // we need to load these as the hql version doesn't remove from the queries and throws
            var logs = _repository.Query<Log>().ToList();
            await _repository.DeleteRange(logs);
        }

        public async Task DeleteLog(int id)
        {
            var log = await _repository.Load(id);
            await _repository.Delete(log);
        }

        public List<SelectListItem> GetSiteOptions()
        {
            IList<Site> sites = _siteRepository.Readonly().OrderBy(site => site.Name).ToList();
            return sites.Count == 1
                ? new List<SelectListItem>()
                : sites
                    .BuildSelectItemList(site => site.Name, site => site.Id.ToString(),
                        emptyItemText: "All sites");
        }

        public IPagedList<Log> GetEntriesPaged(LogSearchQuery searchQuery)
        {
            {
                var query = BaseQuery();
                if (searchQuery.Type.HasValue)
                {
                    query = query.Where(log => log.Type == searchQuery.Type);
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.Message))
                {
                    query =
                        query.Where(
                            log => EF.Functions.Like(searchQuery.Message, $"%{searchQuery.Message}%")
                        );
                }

                if (!string.IsNullOrWhiteSpace(searchQuery.Detail))
                {
                    query = query.Where(
                            log => EF.Functions.Like(searchQuery.Detail, $"%{searchQuery.Detail}%")
                        );
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

                return query.ToPagedList(searchQuery.Page);
            }
        }

        public async Task<IList<Log>> GetAllLogEntries()
        {
            return await BaseQuery().ToListAsync();
        }

        private IQueryable<Log> BaseQuery()
        {
            return
                _repository.Readonly()
                    .OrderByDescending(entry => entry.Id);
        }
    }
}