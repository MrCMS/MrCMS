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
    public class ApiResourceRepository : IApiResourceRepository
    {
        private readonly ISession _session;
        public ApiResourceRepository(ISession session)
        {
            _session = session;
        }
        public async Task<PagedList<ApiResource>> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
           // var pagedList = new PagedList<ApiResource>();
            Expression<Func<ApiResource, bool>> searchCondition = x => x.Name.Contains(search);
            var apiResources = await PagedListExtensions.ToListAsync(_session.Query<ApiResource>().WhereIf(!string.IsNullOrEmpty(search), searchCondition)
                    .OrderBy(x => x.Name).PageBy(x => x.Name, page, pageSize));
          //  apiResources = apiResources.WhereIf(!string.IsNullOrEmpty(search), searchCondition);
         // return new Task<PagedList<ApiResource>>(function: null, Task.FromResult(apiResources).CreationOptions);
         return new PagedList<ApiResource>(apiResources, page, pageSize);
        }

        public Task<ApiResource> GetApiResourceAsync(int apiResourceId)
        {
            return (_session.Query<ApiResource>().Where(x => x.Id == apiResourceId)).FetchMany(x => x.UserClaims).SingleOrDefaultAsync();

           // return _session.GetAsync<ApiResource>(apiResourceId);
          // return result;
        }

        public async Task<PagedList<ApiResourceProperty>> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var properties = await (_session.Query<ApiResourceProperty>().Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Id, page, pageSize)).ToListAsync();
            return new PagedList<ApiResourceProperty>(properties, page, pageSize);
        }

        public Task<ApiResourceProperty> GetApiResourcePropertyAsync(int apiResourcePropertyId)
        {
            return (_session.Query<ApiResourceProperty>().Where(x => x.Id == apiResourcePropertyId).Fetch(x => x.ApiResource).SingleOrDefaultAsync());
        }

        public async Task<int> AddApiResourcePropertyAsync(int apiResourceId, ApiResourceProperty apiResourceProperty)
        {
           var result = 0;
           var apiResource = await (_session.Query<ApiResource>().Where(x => x.Id == apiResourceId).SingleOrDefaultAsync());
           apiResourceProperty.ApiResource = apiResource;
           _session.Transact(session =>
           {
               session.SaveOrUpdate(apiResourceProperty);
               result = apiResourceProperty.Id;
           });

           return result;
        }

        public async Task<int> DeleteApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
        {
            var result = 0;
            var propertyToDelete = await _session.Query<ApiResourceProperty>().Where(x => apiResourceProperty != null && x.Id == apiResourceProperty.Id).SingleOrDefaultAsync();
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

        public async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourceProperty apiResourceProperty)
        {
            var existsWithSameName = await (_session.Query<ApiResourceProperty>().Where(x => x.Key == apiResourceProperty.Key && x.ApiResource.Id == apiResourceProperty.ApiResource.Id).SingleOrDefaultAsync());
            return existsWithSameName == null;
        }

        public Task<int> AddApiResourceAsync(ApiResource apiResource)
        {
            var result = 0;
            _session.Transact(async session =>
            {
               await session.SaveAsync(apiResource);
                result = apiResource.Id;
            });

            return Task.FromResult(result);
        }


        public async Task<int> UpdateApiResourceAsync(ApiResource apiResource)
        {
            var result = 0;
            //Remove old relations and Update with new Data
            var apiResourceClaims = await (_session.Query<ApiResourceClaim>().Where(x => x.ApiResource.Id == apiResource.Id).ToListAsync());
            _session.Transact(session =>
            {
                foreach (var claim in apiResourceClaims)
                {
                    session.Delete(claim);
                }

                session.Update(apiResource);
                result = 1;
            });

            return result;
        }

        public Task<int> DeleteApiResourceAsync(ApiResource apiResource)
        {
            var result = 0;
            _session.Transact(async session =>
            {
                if (apiResource != null && apiResource.Id > 0)
                {
                   await session.DeleteAsync(apiResource);
                    result = 1;
                }
            });

            return Task.FromResult(result);
        }

        public async Task<bool> CanInsertApiResourceAsync(ApiResource apiResource)
        {
            if (apiResource.Id == 0)
            {
                 var existsWithSameName = await _session.QueryOver<ApiResource>().Where(x => x.Name == apiResource.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await _session.QueryOver<ApiResource>().Where(x => x.Name == apiResource.Name && x.Id != apiResource.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        public async Task<PagedList<ApiScope>> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiScopes = await (_session.Query<ApiScope>().Where(x => x.ApiResource.Id == apiResourceId).Fetch(x => x.ApiResource).PageBy(x => x.Id, page, pageSize).ToListAsync());
            return new PagedList<ApiScope>(apiScopes, page, pageSize);
        }

        public Task<ApiScope> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            return _session.Query<ApiScope>()
                .Where(x => x.Id == apiScopeId && x.ApiResource.Id == apiResourceId)
                .Fetch(x => x.ApiResource)
                .FetchMany(x => x.UserClaims)
                .SingleOrDefaultAsync();
        }

        public async Task<int> AddApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            var result = 0;
            var apiResource = await (_session.Query<ApiResource>().Where(x => x.Id == apiResourceId).SingleOrDefaultAsync());
            apiScope.ApiResource = apiResource;
            _session.Transact(session =>
            {
                session.Save(apiScope);
                result = apiScope.Id;
            });

            return result;
        }

        public async Task<int> UpdateApiScopeAsync(int apiResourceId, ApiScope apiScope)
        {
            var result = 0;

            var apiResource = await (_session.Query<ApiResource>().Where(x => x.Id == apiResourceId).SingleOrDefaultAsync());
            apiScope.ApiResource = apiResource;
            //Remove old relations and Update with new Data
            var apiScopeClaims = await (_session.Query<ApiScopeClaim>().Where(x => x.ApiScope.Id == apiScope.Id).ToListAsync());
            _session.Transact(session =>
            {
                foreach (var claim in apiScopeClaims)
                {
                    session.Delete(claim);
                }

                session.Update(apiScope);
                result = 1;
            });

            return result;
        }

        public Task<int> DeleteApiScopeAsync(ApiScope apiScope)
        {
            var result = 0;
            _session.Transact(async session =>
            {
                if (apiScope != null && apiScope.Id > 0)
                {
                    await session.DeleteAsync(apiScope);
                    result = 1;
                }
            });

            return Task.FromResult(result);
        }

        public async Task<PagedList<ApiSecret>> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiSecrets = await(_session.Query<ApiSecret>().Where(x => x.ApiResource.Id == apiResourceId).PageBy(x => x.Id, page, pageSize).ToListAsync());
            return new PagedList<ApiSecret>(apiSecrets, page, pageSize);
        }

        public async Task<int> AddApiSecretAsync(int apiResourceId, ApiSecret apiSecret)
        {
            var result = 0;
            var apiResource = await(_session.Query<ApiResource>().Where(x => x.Id == apiResourceId).SingleOrDefaultAsync());
            apiSecret.ApiResource = apiResource;
            _session.Transact(session =>
            {
                session.Save(apiSecret);
                result = apiSecret.Id;
            });

            return result;
        }

        public Task<ApiSecret> GetApiSecretAsync(int apiSecretId)
        {
            return _session.Query<ApiSecret>()
                .Where(x => x.Id == apiSecretId)
                .Fetch(x => x.ApiResource)
                .SingleOrDefaultAsync();
        }

        public Task<int> DeleteApiSecretAsync(ApiSecret apiSecret)
        {
            var result = 0;
            _session.Transact(async session =>
            {
                if (apiSecret != null && apiSecret.Id > 0)
                {
                    await session.DeleteAsync(apiSecret);
                    result = 1;
                }
            });

            return Task.FromResult(result);
        }

        public async Task<bool> CanInsertApiScopeAsync(ApiScope apiScope)
        {
            if (apiScope.Id == 0)
            {
                var existsWithSameName = await _session.QueryOver<ApiScope>().Where(x => x.Name == apiScope.Name).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
            else
            {
                var existsWithSameName = await _session.QueryOver<ApiScope>().Where(x => x.Name == apiScope.Name && x.Id != apiScope.Id).SingleOrDefaultAsync();
                return existsWithSameName == null;
            }
        }

        public Task<int> SaveAllChangesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            return _session.Query<ApiResource>()
                .Where(x => x.Id == apiResourceId).Select(x => x.Name)
                .SingleOrDefaultAsync();
        }
    }
}