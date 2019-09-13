using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MrCMS.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions;
using NHibernate;
using NHibernate.Linq;
using X.PagedList;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Repositories
{
    public class IdentityResourceRepository : IIdentityResourceRepository
    {
        private readonly ISession _session;
        public IdentityResourceRepository(ISession session)
        {
            _session = session;
        }
        public async Task<PagedList<IdentityResource>> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            Expression<Func<IdentityResource, bool>> searchCondition = x => x.Name.Contains(search);
            var clients = await _session.Query<IdentityResource>().WhereIf(!string.IsNullOrEmpty(search), searchCondition)
                .PageBy(x => x.Name, page, pageSize).ToListAsync();
            return new PagedList<IdentityResource>(clients, page, pageSize);
        }

        public Task<IdentityResource> GetIdentityResourceAsync(int identityResourceId)
        {
            return (_session.Query<IdentityResource>()
                .Where(x => x.Id == identityResourceId))
                .FetchMany(x => x.UserClaims)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> CanInsertIdentityResourceAsync(IdentityResource identityResource)
        {
            if (identityResource.Id == 0)
            {
                var existsWithSameName = await _session.QueryOver<IdentityResource>().Where(x => x.Name == identityResource.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await _session.QueryOver<ApiResource>().Where(x => x.Name == identityResource.Name && x.Id != identityResource.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        public Task<int> AddIdentityResourceAsync(IdentityResource identityResource)
        {
            var result = 0;
            _session.Transact(async session =>
            {
                await session.SaveAsync(identityResource);
                result = identityResource.Id;
            });

            return Task.FromResult(result);
        }

        public async Task<int> UpdateIdentityResourceAsync(IdentityResource identityResource)
        {
            var result = 0;
            //Remove old relations and Update with new Data
            var identityClaims = await(_session.Query<IdentityClaim>().Where(x => x.IdentityResource.Id == identityResource.Id).ToListAsync());
            _session.Transact(session =>
            {
                foreach (var claim in identityClaims)
                {
                    session.Delete(claim);
                }

                session.Update(identityResource);
                result = 1;
            });

            return result;
        }

        public async Task<int> DeleteIdentityResourceAsync(IdentityResource identityResource)
        {
            var result = 0;
            var identityResourceToDelete = await _session.Query<IdentityResource>().Where(x => identityResource != null && x.Id == identityResource.Id).SingleOrDefaultAsync();
            _session.Transact(session =>
            {
                if (identityResourceToDelete != null && identityResourceToDelete.Id > 0)
                {
                    session.DeleteAsync(identityResourceToDelete);
                    result = 1;
                }
            });

            return result;
        }

        public async Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourceProperty identityResourceProperty)
        {
            var existsWithSameName = await(_session.Query<IdentityResourceProperty>().Where(x => x.Key == identityResourceProperty.Key 
                                                                                                 && x.IdentityResource.Id == identityResourceProperty.IdentityResource.Id).SingleOrDefaultAsync());
            return existsWithSameName == null;
        }

        public async Task<PagedList<IdentityResourceProperty>> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1, int pageSize = 10)
        {
            var properties = await(_session.Query<IdentityResourceProperty>().Where(x => x.IdentityResource.Id == identityResourceId).PageBy(x => x.Id, page, pageSize)).ToListAsync();
            return new PagedList<IdentityResourceProperty>(properties, page, pageSize);
        }

        public Task<IdentityResourceProperty> GetIdentityResourcePropertyAsync(int identityResourcePropertyId)
        {
            return (_session.Query<IdentityResourceProperty>()
                .Where(x => x.Id == identityResourcePropertyId)
                .Fetch(x => x.IdentityResource)
                .SingleOrDefaultAsync());
        }

        public async Task<int> AddIdentityResourcePropertyAsync(int identityResourceId, IdentityResourceProperty identityResourceProperty)
        {
            var result = 0;
            var identityResource = await(_session.Query<IdentityResource>().Where(x => x.Id == identityResourceId).SingleOrDefaultAsync());
            identityResourceProperty.IdentityResource = identityResource;
            _session.Transact(session =>
            {
                session.SaveOrUpdate(identityResourceProperty);
                result = identityResourceProperty.Id;
            });

            return result;
        }

        public async Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourceProperty identityResourceProperty)
        {
            var result = 0;
            var propertyToDelete = await _session.Query<IdentityResourceProperty>().Where(x => identityResourceProperty != null && x.Id == identityResourceProperty.Id).SingleOrDefaultAsync();
            _session.Transact(session =>
            {
                if (propertyToDelete != null && propertyToDelete.Id > 0)
                {
                    session.DeleteAsync(propertyToDelete);
                    result = 1;
                }
            });

            return result;
        }

        public Task<int> SaveAllChangesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}