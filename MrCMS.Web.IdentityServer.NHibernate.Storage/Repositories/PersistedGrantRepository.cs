using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Models;
using NHibernate;
using NHibernate.Linq;
using X.PagedList;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Repositories
{
    public class PersistedGrantRepository : IPersistedGrantRepository
    {
        private readonly ISession _session;
        public PersistedGrantRepository(ISession session)
        {
            _session = session;
        }
        public async Task<PagedList<PersistedGrantDataView>> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var persistedGrantByUsers = _session.Query<PersistedGrant>()
                .Select(x => new PersistedGrantDataView()
                {
                    SubjectId = x.SubjectId,
                    SubjectName = string.Empty
                })
                .Distinct();

            Expression<Func<PersistedGrantDataView, bool>> searchCondition = x => x.SubjectId.Contains(search);
            var persistedGrantsData = await persistedGrantByUsers.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.SubjectId, page, pageSize).ToListAsync();
            return new PagedList<PersistedGrantDataView>(persistedGrantsData, page, pageSize);
        }

        public async Task<PagedList<PersistedGrant>> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
        {
            var persistedGrantsData = await _session.Query<PersistedGrant>()
                .Where(x => x.SubjectId == subjectId)
                .Select(x => new PersistedGrant()
                {
                    SubjectId = x.SubjectId,
                    Type = x.Type,
                    Key = x.Key,
                    ClientId = x.ClientId,
                    Data = x.Data,
                    Expiration = x.Expiration,
                    CreationTime = x.CreationTime
                })
                .PageBy(x => x.SubjectId, page, pageSize)
                .ToListAsync();
            return new PagedList<PersistedGrant>(persistedGrantsData, page, pageSize);
        }

        public Task<PersistedGrant> GetPersistedGrantAsync(string key)
        {
            return _session.Query<PersistedGrant>().SingleOrDefaultAsync(x => x.Key == key);
        }

        public async Task<int> DeletePersistedGrantAsync(string key)
        {
            var result = 0;
            var persistedGrant = await _session.Query<PersistedGrant>().Where(x => x.Key == key).SingleOrDefaultAsync();
            _session.Transact(session =>
            {
                if (persistedGrant != null && persistedGrant.Id > 0)
                {
                    session.DeleteAsync(persistedGrant);
                    result = 1;
                }
            });

            return result;
        }

        public async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            var result = 0;
            var grants = await _session.Query<PersistedGrant>().Where(x => x.SubjectId == userId).ToListAsync();
            _session.Transact(session =>
            {
                foreach (var grant in grants)
                {
                    session.DeleteAsync(grant);
                }

                result = 1;
            });

            return result;
        }

        public Task<bool> ExistsPersistedGrantsAsync(string subjectId)
        {
            return _session.Query<PersistedGrant>().AnyAsync(x => x.SubjectId == subjectId);
        }

        public Task<int> SaveAllChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}