using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.Models;
using MrCMS.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Constants;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Helpers;
using NHibernate;
using NHibernate.Linq;
using X.PagedList;
using Client = MrCMS.Web.IdentityServer.NHibernate.Storage.Entities.Client;
using IdentityResource = MrCMS.Web.IdentityServer.NHibernate.Storage.Entities.IdentityResource;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Repositories
{
    public  class ClientRepository : IClientRepository
    {
        private readonly ISession _session;
        public ClientRepository(ISession session)
        {
            _session = session;
        }
        /// <summary>
        /// Add new client, this method doesn't save client secrets, client claims, client properties
        /// </summary>
        /// <param name="client"></param>
        /// <returns>This method return new client id</returns>
        public Task<int> AddClientAsync(Client client)
        {
            var result = 0;
            _session.Transact(session =>
            {
                session.Save(client);
                result = client.Id;
            });

            return Task.FromResult(result);
        }

        public async Task<int> UpdateClientAsync(Client client)
        {
            var result = 0;
            //Remove old relations and Update with new Data
            var clientScopes = await(_session.Query<ClientScope>().Where(x => x.Client.Id == client.Id).ToListAsync());

            var clientGrantTypes = await (_session.Query<ClientGrantType>().Where(x => x.Client.Id == client.Id).ToListAsync());

            var clientRedirectUris = await (_session.Query<ClientRedirectUri>().Where(x => x.Client.Id == client.Id).ToListAsync());

            var clientCorsOrigins = await (_session.Query<ClientCorsOrigin>().Where(x => x.Client.Id == client.Id).ToListAsync());

            var clientIdPRestrictions = await (_session.Query<ClientIdPRestriction>().Where(x => x.Client.Id == client.Id).ToListAsync());

            var clientPostLogoutRedirectUris = await (_session.Query<ClientPostLogoutRedirectUri>().Where(x => x.Client.Id == client.Id).ToListAsync());


            _session.Transact(session =>
            {
                // Remove old allowed scopes
                foreach (var scope in clientScopes)
                {
                    session.Delete(scope);
                }

                // Remove old grant types
                foreach (var grant in clientGrantTypes)
                {
                    session.Delete(grant);
                }

                // Remove old redirect uri
                foreach (var redirect in clientRedirectUris)
                {
                    session.Delete(redirect);
                }
                //Remove old client cors
                foreach (var cors in clientCorsOrigins)
                {
                    session.Delete(cors);
                }
                //Remove old client id restrictions
                foreach (var restriction in clientIdPRestrictions)
                {
                    session.Delete(restriction);
                }
                //Remove old client post logout redirect
                foreach (var postredirect in clientPostLogoutRedirectUris)
                {
                    session.Delete(postredirect);
                }
                session.Update(client);
                result = 1;
            });

            return result;
        }

        public async Task<int> RemoveClientAsync(Client client)
        {
            var result = 0;
            var clientToDelete = await _session.Query<Client>().Where(x => client != null && x.Id == client.Id).SingleOrDefaultAsync();
            _session.Transact(session =>
            {
                if (clientToDelete != null && clientToDelete.Id > 0)
                {
                    session.DeleteAsync(clientToDelete);
                    result = 1;
                }
            });

            return result;
        }

        public async Task<int> CloneClientAsync(Client client, bool cloneClientCorsOrigins = true, bool cloneClientGrantTypes = true,
            bool cloneClientIdPRestrictions = true, bool cloneClientPostLogoutRedirectUris = true,
            bool cloneClientScopes = true, bool cloneClientRedirectUris = true, bool cloneClientClaims = true,
            bool cloneClientProperties = true)
        {
            var result = 0;
            var clientToClone = await _session.Query<Client>()
                .FetchMany(x => x.AllowedGrantTypes)
                .FetchMany(x => x.RedirectUris)
                .FetchMany(x => x.PostLogoutRedirectUris)
                .FetchMany(x => x.AllowedScopes)
                .FetchMany(x => x.ClientSecrets)
                .FetchMany(x => x.Claims)
                .FetchMany(x => x.IdentityProviderRestrictions)
                .FetchMany(x => x.AllowedCorsOrigins)
                .FetchMany(x => x.Properties)
                .FirstOrDefaultAsync(x => x.Id == client.Id);

            clientToClone.ClientName = client.ClientName;
            clientToClone.ClientId = client.ClientId;

            //Clean original ids
            clientToClone.Id = 0;
            clientToClone.AllowedCorsOrigins.ForEach(x => x.Id = 0);
            clientToClone.RedirectUris.ForEach(x => x.Id = 0);
            clientToClone.PostLogoutRedirectUris.ForEach(x => x.Id = 0);
            clientToClone.AllowedScopes.ForEach(x => x.Id = 0);
            clientToClone.ClientSecrets.ForEach(x => x.Id = 0);
            clientToClone.IdentityProviderRestrictions.ForEach(x => x.Id = 0);
            clientToClone.Claims.ForEach(x => x.Id = 0);
            clientToClone.AllowedGrantTypes.ForEach(x => x.Id = 0);
            clientToClone.Properties.ForEach(x => x.Id = 0);

            //Client secret will be skipped
            clientToClone.ClientSecrets.Clear();

            if (!cloneClientCorsOrigins)
            {
                clientToClone.AllowedCorsOrigins.Clear();
            }

            if (!cloneClientGrantTypes)
            {
                clientToClone.AllowedGrantTypes.Clear();
            }

            if (!cloneClientIdPRestrictions)
            {
                clientToClone.IdentityProviderRestrictions.Clear();
            }

            if (!cloneClientPostLogoutRedirectUris)
            {
                clientToClone.PostLogoutRedirectUris.Clear();
            }

            if (!cloneClientScopes)
            {
                clientToClone.AllowedScopes.Clear();
            }

            if (!cloneClientRedirectUris)
            {
                clientToClone.RedirectUris.Clear();
            }

            if (!cloneClientClaims)
            {
                clientToClone.Claims.Clear();
            }

            if (!cloneClientProperties)
            {
                clientToClone.Properties.Clear();
            }

            _session.Transact(session =>
            {
                session.Save(clientToClone);
                result = clientToClone.Id;
            });

            return result;
        }

        public async Task<bool> CanInsertClientAsync(Client client, bool isCloned = false)
        {
            if (client.Id == 0 || isCloned)
            {
                var existsWithClientName = await _session.Query<Client>().Where(x => x.ClientId == client.ClientId).SingleOrDefaultAsync();
                return existsWithClientName == null;
            }
            else
            {
                var existsWithClientName = await _session.Query<Client>().Where(x => x.ClientId == client.ClientId && x.Id != client.Id).SingleOrDefaultAsync();
                return existsWithClientName == null;
            }
        }

        public Task<Client> GetClientAsync(int clientId)
        {
            return _session.Query<Client>().Where(x => x.Id == clientId)
                .FetchMany(x => x.AllowedGrantTypes)
                .FetchMany(x => x.RedirectUris)
                .FetchMany(x => x.PostLogoutRedirectUris)
                .FetchMany(x => x.AllowedScopes)
                .FetchMany(x => x.ClientSecrets)
                .FetchMany(x => x.Claims)
                .FetchMany(x => x.IdentityProviderRestrictions)
                .FetchMany(x => x.AllowedCorsOrigins)
                .FetchMany(x => x.Properties)
                .SingleOrDefaultAsync();
        }

        public async Task<(string ClientId, string ClientName)> GetClientIdAsync(int clientId)
        {
            var client = await(_session.Query<Client>().Where(x => x.Id == clientId)
                .Select(x => new { x.ClientId, x.ClientName })
                .SingleOrDefaultAsync());

            return (client?.ClientId, client?.ClientName);
        }

        public async Task<PagedList<Client>> GetClientsAsync(string search = "", int page = 1, int pageSize = 10)
        {
            Expression<Func<Client, bool>> searchCondition = x => x.ClientId.Contains(search) || x.ClientName.Contains(search);
            var clients = await _session.Query<Client>().WhereIf(!string.IsNullOrEmpty(search), searchCondition)
                .PageBy(x => x.Id, page, pageSize).ToListAsync();
            return new PagedList<Client>(clients, page, pageSize);
        }

        public async Task<List<string>> GetScopesAsync(string scope, int limit = 0)
        {
            var identityResources = await _session.Query<IdentityResource>()
                .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
                .TakeIf(x => x.Id, limit > 0, limit)
                .Select(x => x.Name)
                .ToListAsync();

            var apiScopes = await _session.Query<ApiScope>()
                .WhereIf(!string.IsNullOrEmpty(scope), x => x.Name.Contains(scope))
                .TakeIf(x => x.Id, limit > 0, limit)
                .Select(x => x.Name)
                .ToListAsync();

            var scopes = identityResources.Concat(apiScopes).TakeIf(x => x, limit > 0, limit).ToList();

            return scopes;
        }

        public List<string> GetGrantTypes(string grant, int limit = 0)
        {
            var filteredGrants = ClientConsts.GetGrantTypes()
                .WhereIf(!string.IsNullOrWhiteSpace(grant), x => x.Contains(grant))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredGrants;
        }

        public List<SelectItem> GetProtocolTypes()
        {
            return ClientConsts.GetProtocolTypes();
        }

        public List<SelectItem> GetAccessTokenTypes()
        {
            var accessTokenTypes = EnumHelpers.ToSelectList<AccessTokenType>();
            return accessTokenTypes;
        }

        public List<SelectItem> GetTokenExpirations()
        {
            var tokenExpirations = EnumHelpers.ToSelectList<TokenExpiration>();
            return tokenExpirations;
        }

        public List<SelectItem> GetTokenUsage()
        {
            var tokenUsage = EnumHelpers.ToSelectList<TokenUsage>();
            return tokenUsage;
        }

        public List<SelectItem> GetHashTypes()
        {
            var hashTypes = EnumHelpers.ToSelectList<HashType>();
            return hashTypes;
        }

        public List<SelectItem> GetSecretTypes()
        {
            var secrets = new List<SelectItem>();
            secrets.AddRange(ClientConsts.GetSecretTypes().Select(x => new SelectItem(x, x)));

            return secrets;
        }

        public List<string> GetStandardClaims(string claim, int limit = 0)
        {
            var filteredClaims = ClientConsts.GetStandardClaims()
                .WhereIf(!string.IsNullOrWhiteSpace(claim), x => x.Contains(claim))
                .TakeIf(x => x, limit > 0, limit)
                .ToList();

            return filteredClaims;
        }

        public async Task<int> AddClientSecretAsync(int clientId, ClientSecret clientSecret)
        {
            var result = 0;
            var client = await(_session.Query<Client>().Where(x => x.Id == clientId).SingleOrDefaultAsync());
            clientSecret.Client = client;
            _session.Transact(session =>
            {
                session.Save(clientSecret);
                result = clientSecret.Id;
            });

            return result;
        }

        public async Task<int> DeleteClientSecretAsync(ClientSecret clientSecret)
        {
            var result = 0;
            var secretToDelete = await _session.Query<ClientSecret>().Where(x => clientSecret != null && x.Id == clientSecret.Id).SingleOrDefaultAsync();
            _session.Transact(session =>
            {
                if (secretToDelete != null && secretToDelete.Id > 0)
                {
                    session.DeleteAsync(secretToDelete);
                    result = 1;
                }
            });

            return result;
        }

        public async Task<PagedList<ClientSecret>> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var secrets = await _session.Query<ClientSecret>()
                .Where(x => x.Client.Id == clientId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PagedList<ClientSecret>(secrets, page, pageSize);
        }

        public Task<ClientSecret> GetClientSecretAsync(int clientSecretId)
        {
            return _session.Query<ClientSecret>()
                .Where(x => x.Id == clientSecretId)
                .Fetch(x => x.Client)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<ClientClaim>> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var claims = await _session.Query<ClientClaim>()
                .Where(x => x.Client.Id == clientId)
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();
            return new PagedList<ClientClaim>(claims, page, pageSize);
        }

        public async Task<PagedList<ClientProperty>> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10)
        {
            var properties = await _session.Query<ClientProperty>()
                .Where(x => x.Client.Id == clientId)
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();
            return new PagedList<ClientProperty>(properties, page, pageSize);
        }

        public Task<ClientClaim> GetClientClaimAsync(int clientClaimId)
        {
            return _session.Query<ClientClaim>()
                .Where(x => x.Id == clientClaimId)
                .Fetch(x => x.Client)
                .SingleOrDefaultAsync();
        }

        public Task<ClientProperty> GetClientPropertyAsync(int clientPropertyId)
        {
            return (_session.Query<ClientProperty>()
                .Where(x => x.Id == clientPropertyId)
                .Fetch(x => x.Client)
                .SingleOrDefaultAsync());
        }

        public async Task<int> AddClientClaimAsync(int clientId, ClientClaim clientClaim)
        {
            var result = 0;
            var client = await(_session.Query<Client>().Where(x => x.Id == clientId).SingleOrDefaultAsync());
            clientClaim.Client = client;
            _session.Transact(session =>
            {
                session.Save(clientClaim);
                result = clientClaim.Id;
            });

            return result;
        }

        public async Task<int> AddClientPropertyAsync(int clientId, ClientProperty clientProperty)
        {
            var result = 0;
            var client = await _session.Query<Client>().Where(x => x.Id == clientId).SingleOrDefaultAsync();
            clientProperty.Client = client;
            _session.Transact(session =>
            {
                session.Save(clientProperty);
                result = clientProperty.Id;
            });

            return result;
        }

        public async  Task<int> DeleteClientClaimAsync(ClientClaim clientClaim)
        {
            var result = 0;
            var claimToDelete = await _session.Query<ClientClaim>().Where(x => clientClaim != null && x.Id == clientClaim.Id).SingleOrDefaultAsync();
            _session.Transact(session =>
            {
                if (claimToDelete != null && claimToDelete.Id > 0)
                {
                    session.DeleteAsync(claimToDelete);
                    result = 1;
                }
            });

            return result;
        }

        public async Task<int> DeleteClientPropertyAsync(ClientProperty clientProperty)
        {
            var result = 0;
            var propertyToDelete = await _session.Query<ClientProperty>().Where(x => clientProperty != null && x.Id == clientProperty.Id).SingleOrDefaultAsync();
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
            throw new NotImplementedException();
        }
    }
}