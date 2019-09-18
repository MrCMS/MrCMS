using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Common;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions;
using X.PagedList;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services
{
    public interface IClientService
    {
        ClientDto BuildClientViewModel(ClientDto client = null);

        ClientSecretsDto BuildClientSecretsViewModel(ClientSecretsDto clientSecrets);

        ClientCloneDto BuildClientCloneViewModel(int id, ClientDto clientDto);

        Task<int> AddClientAsync(ClientDto client);

        Task<int> UpdateClientAsync(ClientDto client);

        Task<int> RemoveClientAsync(ClientDto client);

        Task<int> RemoveClientAsync(int clientId);

        Task<int> CloneClientAsync(ClientCloneDto client);

        Task<bool> CanInsertClientAsync(ClientDto client, bool isCloned = false);

        Task<ClientDto> GetClientAsync(int clientId);

        Task<ClientsDto> GetClientsAsync(string search, int page = 1, int pageSize = 10);

        Task<List<string>> GetScopesAsync(string scope, int limit = 0);

        List<string> GetGrantTypes(string grant, int limit = 0);

        List<SelectItemDto> GetAccessTokenTypes();

        List<SelectItemDto> GetTokenExpirations();

        List<SelectItemDto> GetTokenUsage();

        List<SelectItemDto> GetHashTypes();

        List<SelectItemDto> GetSecretTypes();

        List<string> GetStandardClaims(string claim, int limit = 0);

        Task<int> AddClientSecretAsync(ClientSecretsDto clientSecret);

        Task<int> DeleteClientSecretAsync(ClientSecretsDto clientSecret);

        Task<int> DeleteClientSecretAsync(int clientSecretId);

        Task<ClientSecretsDto> GetClientSecretsAsync(int clientId, int page = 1, int pageSize = 10);

        Task<ClientSecretsDto> GetClientSecretAsync(int clientSecretId);

        Task<ClientClaimsDto> GetClientClaimsAsync(int clientId, int page = 1, int pageSize = 10);

        Task<ClientPropertiesDto> GetClientPropertiesAsync(int clientId, int page = 1, int pageSize = 10);

        Task<ClientClaimsDto> GetClientClaimAsync(int clientClaimId);

        Task<ClientPropertiesDto> GetClientPropertyAsync(int clientPropertyId);

        Task<int> AddClientClaimAsync(ClientClaimsDto clientClaim);

        Task<int> AddClientPropertyAsync(ClientPropertiesDto clientProperties);

        Task<int> DeleteClientClaimAsync(ClientClaimsDto clientClaim);

        Task<int> DeleteClientClaimAsync(int clientClaimId);

        Task<int> DeleteClientPropertyAsync(ClientPropertiesDto clientProperty);

        Task<int> DeleteClientPropertyAsync(int clientPropertyId);

        List<SelectItemDto> GetProtocolTypes();
    }
}
