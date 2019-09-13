using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Stores
{
    /// <summary>
    /// Implementation of the NHibernate-based IResourceStore.
    /// </summary>
    public class ResourceStore : IResourceStore
    {
        private readonly ISession _session;
        private readonly ILogger<ResourceStore> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="session">The NHibernate session used to retrieve the data.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="mapper">The Mapper.</param>
        public ResourceStore(ISession session, ILogger<ResourceStore> logger, IMapper mapper)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Finds the API resource by name.
        /// </summary>
        public async Task<IdentityServer4.Models.ApiResource> FindApiResourceAsync(string name)
        {
            var apiResource = await _session.QueryOver<Entities.ApiResource>()
                .Where(r => r.Name == name)
                .Fetch(SelectMode.Fetch, r => r.Secrets)
                .Fetch(SelectMode.Fetch, r => r.Scopes)
                .Fetch(SelectMode.Fetch, r => r.UserClaims)
                .SingleOrDefaultAsync();

            if (apiResource != null)
            {
                _logger.LogDebug("Found {api} API resource in database", name);
            }
            else
            {
                _logger.LogDebug("Did not find {api} API resource in database", name);
            }

            return _mapper.Map<IdentityServer4.Models.ApiResource>(apiResource);
           // return apiResource.ToModel();
        }

        /// <summary>
        /// Gets API resources by scope names.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public async Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            ApiScopeClaim apiScopeClaimAlias = null;
            var resourcesQuery = _session.QueryOver<Entities.ApiResource>()
                .Fetch(SelectMode.Fetch, api => api.Secrets)
                .Fetch(SelectMode.Fetch, api => api.UserClaims)
                .Fetch(SelectMode.Fetch, api => api.Properties)
                // Left specification is mandatory for NHibernate to eagerly fetch the associations
                .Left.JoinQueryOver<ApiScope>(api => api.Scopes) 
                    .Left.JoinAlias(scope => scope.UserClaims, () => apiScopeClaimAlias)
                    .Where(scope => scope.Name.IsIn(scopeNames.ToArray()))
                .TransformUsing(Transformers.DistinctRootEntity);

            var results = await resourcesQuery.ListAsync();

           // _mapper.ProjectTo<>()
            var models = results.Select(x => _mapper.Map<IdentityServer4.Models.ApiResource>(x)).ToArray();

          
            _logger.LogDebug("Found {scopes} API scopes in database", models.SelectMany(x => x.Scopes).Select(x => x.Name));

            return models;
        }

        /// <summary>
        /// Gets identity resources by scope names.
        /// </summary>
        /// <param name="scopeNames">Scope name/names.</param>
        public async Task<IEnumerable<IdentityServer4.Models.IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var resourcesQuery = _session.QueryOver<Entities.IdentityResource>()
                .Where(r => r.Name.IsIn(scopeNames.ToArray()))
                .TransformUsing(Transformers.DistinctRootEntity);

            var results = await resourcesQuery.ListAsync();

            _logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return results.Select(x => _mapper.Map<IdentityServer4.Models.IdentityResource>(x)).ToArray();
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        public async Task<Resources> GetAllResourcesAsync()
        {
            Resources result = null;
            using (var tx = _session.BeginTransaction())
            {
                var identityResources = _session.QueryOver<Entities.IdentityResource>()
                    .Fetch(SelectMode.Fetch, ir => ir.UserClaims)
                    .Fetch(SelectMode.Fetch, ir => ir.Properties)
                    .Future();

                var apiResources = _session.QueryOver<Entities.ApiResource>()
                    .Fetch(SelectMode.Fetch, ar => ar.Secrets)
                    .Fetch(SelectMode.Fetch, ar => ar.Scopes)
                    .Fetch(SelectMode.Fetch, ar => ar.UserClaims)
                    .Fetch(SelectMode.Fetch, ar => ar.Properties)
                    .Future();

                result = new Resources(
                    (await identityResources.GetEnumerableAsync())
                        .Select(identity => _mapper.Map<IdentityServer4.Models.IdentityResource>(identity))
                        .ToArray(),
                    (await apiResources.GetEnumerableAsync())
                        .Select(api => _mapper.Map<IdentityServer4.Models.ApiResource>(api))
                        .ToArray()
                );

                await tx.CommitAsync();
            }

            _logger.LogDebug("Found {scopes} as all scopes in database", 
                result.IdentityResources.Select(identity => identity.Name)
                    .Union(result.ApiResources.SelectMany(api => api.Scopes)
                .Select(scope => scope.Name)));

            return await Task.FromResult(result);
        }
    }
}