using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using X.PagedList;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services
{
   public interface IApiResourceService
    {
        ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets);

        ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope);

        Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId);

        Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties);

        Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

        Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty);

        Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId);

        Task<int> AddApiResourceAsync(ApiResourceDto apiResource);

        Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource);

        Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource);

        Task<int> DeleteApiResourceAsync(int apiResourceId);

        Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource);

        Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId);

        Task<int> AddApiScopeAsync(ApiScopesDto apiScope);

        Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope);

        Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope);

        Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10);

        Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret);

        Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId);

        Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret);

        Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes);

        Task<string> GetApiResourceNameAsync(int apiResourceId);
    }
}
