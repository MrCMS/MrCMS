using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.Models;
using MrCMS.Helpers;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.ExceptionHandling;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Helpers;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Mappers;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Resources;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Helpers;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Repositories;
using NHibernate;
using NHibernate.Linq;
using X.PagedList;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services
{
    public class ApiResourceService : IApiResourceService
    {
        protected readonly IApiResourceRepository ApiResourceRepository;
        protected readonly IApiResourceServiceResources ApiResourceServiceResources;
        protected readonly IClientService ClientService;
        private const string SharedSecret = "SharedSecret";



        public ApiResourceService(IApiResourceRepository apiResourceRepository,
            IApiResourceServiceResources apiResourceServiceResources,
            IClientService clientService)
        {
            ApiResourceRepository = apiResourceRepository;
            ApiResourceServiceResources = apiResourceServiceResources;
            ClientService = clientService;
        }

        private void HashApiSharedSecret(ApiSecretsDto apiSecret)
        {
            if (apiSecret.Type != SharedSecret) return;

            if (apiSecret.HashType == ((int)HashType.Sha256).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha256();
            }
            else if (apiSecret.HashType == ((int)HashType.Sha512).ToString())
            {
                apiSecret.Value = apiSecret.Value.Sha512();
            }
        }

        private async Task BuildApiScopesViewModelAsync(ApiScopesDto apiScope)
        {
            if (apiScope.ApiScopeId == 0)
            {
                var apiScopesDto = await GetApiScopesAsync(apiScope.ApiResourceId);
                apiScope.Scopes.AddRange(apiScopesDto.Scopes);
                apiScope.TotalCount = apiScopesDto.TotalCount;
            }
        }

        private async Task BuildApiResourcePropertiesViewModelAsync(ApiResourcePropertiesDto apiResourceProperties)
        {
            var apiResourcePropertiesDto = await GetApiResourcePropertiesAsync(apiResourceProperties.ApiResourceId);
            apiResourceProperties.ApiResourceProperties.AddRange(apiResourcePropertiesDto.ApiResourceProperties);
            apiResourceProperties.TotalCount = apiResourcePropertiesDto.TotalCount;
        }
        public ApiSecretsDto BuildApiSecretsViewModel(ApiSecretsDto apiSecrets)
        {
            apiSecrets.HashTypes = ClientService.GetHashTypes();
            apiSecrets.TypeList = ClientService.GetSecretTypes();

            return apiSecrets;
        }

        public ApiScopesDto BuildApiScopeViewModel(ApiScopesDto apiScope)
        {
            ComboBoxHelpers.PopulateValuesToList(apiScope.UserClaimsItems, apiScope.UserClaims);

            return apiScope;
        }

        public async Task<ApiResourcesDto> GetApiResourcesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await ApiResourceRepository.GetApiResourcesAsync(search, page, pageSize);
            var apiResourcesDto = pagedList.ToModel();

            return apiResourcesDto;
        }

        public async Task<ApiResourcePropertiesDto> GetApiResourcePropertiesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await ApiResourceRepository.GetApiResourcePropertiesAsync(apiResourceId, page, pageSize);
            var apiResourcePropertiesDto = pagedList.ToModel();
            apiResourcePropertiesDto.ApiResourceId = apiResourceId;
            apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            return apiResourcePropertiesDto;
        }

        public async Task<ApiResourcePropertiesDto> GetApiResourcePropertyAsync(int apiResourcePropertyId)
        {
            var apiResourceProperty = await ApiResourceRepository.GetApiResourcePropertyAsync(apiResourcePropertyId);
            if (apiResourceProperty == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourcePropertyDoesNotExist().Description, apiResourcePropertyId));

            var apiResourcePropertiesDto = apiResourceProperty.ToModel();
            apiResourcePropertiesDto.ApiResourceId = apiResourceProperty.ApiResource.Id;
            apiResourcePropertiesDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceProperty.ApiResource.Id);

            return apiResourcePropertiesDto;
        }

        public async Task<int> AddApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperties)
        {
            var canInsert = await CanInsertApiResourcePropertyAsync(apiResourceProperties);
            if (!canInsert)
            {
                await BuildApiResourcePropertiesViewModelAsync(apiResourceProperties);
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourcePropertyExistsValue().Description, apiResourceProperties.Key), ApiResourceServiceResources.ApiResourcePropertyExistsKey().Description, apiResourceProperties);
            }

            var apiResourceProperty = apiResourceProperties.ToEntity();

            return await ApiResourceRepository.AddApiResourcePropertyAsync(apiResourceProperties.ApiResourceId, apiResourceProperty);
        }

        public async Task<int> DeleteApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var propertyEntity = apiResourceProperty.ToEntity();

            return await ApiResourceRepository.DeleteApiResourcePropertyAsync(propertyEntity);
        }

        public async Task<bool> CanInsertApiResourcePropertyAsync(ApiResourcePropertiesDto apiResourceProperty)
        {
            var resource = apiResourceProperty.ToEntity();

            return await ApiResourceRepository.CanInsertApiResourcePropertyAsync(resource);
        }

        public async Task<ApiResourceDto> GetApiResourceAsync(int apiResourceId)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, ApiResourceServiceResources.ApiResourceDoesNotExist().Description);
            var apiResourceDto = apiResource.ToModel();

            return apiResourceDto;
        }

        public async Task<int> AddApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
            }

            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.AddApiResourceAsync(resource);
        }

        public async Task<int> UpdateApiResourceAsync(ApiResourceDto apiResource)
        {
            var canInsert = await CanInsertApiResourceAsync(apiResource);
            if (!canInsert)
            {
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiResourceExistsValue().Description, apiResource.Name), ApiResourceServiceResources.ApiResourceExistsKey().Description, apiResource);
            }

            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.UpdateApiResourceAsync(resource);
        }

        public async Task<int> DeleteApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.DeleteApiResourceAsync(resource);
        }

        public async Task<int> DeleteApiResourceAsync(int apiResourceId)
        {
            var apiResourceEntity = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);

            return await ApiResourceRepository.DeleteApiResourceAsync(apiResourceEntity);
        }

        public async Task<bool> CanInsertApiResourceAsync(ApiResourceDto apiResource)
        {
            var resource = apiResource.ToEntity();

            return await ApiResourceRepository.CanInsertApiResourceAsync(resource);
        }

        public async Task<ApiScopesDto> GetApiScopesAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await ApiResourceRepository.GetApiScopesAsync(apiResourceId, page, pageSize);

            var apiScopesDto = pagedList.ToModel();
            apiScopesDto.ApiResourceId = apiResourceId;
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public async Task<ApiScopesDto> GetApiScopeAsync(int apiResourceId, int apiScopeId)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var apiScope = await ApiResourceRepository.GetApiScopeAsync(apiResourceId, apiScopeId);
            if (apiScope == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiScopeDoesNotExist().Description, apiScopeId), ApiResourceServiceResources.ApiScopeDoesNotExist().Description);

            var apiScopesDto = apiScope.ToModel();
            apiScopesDto.ResourceName = await GetApiResourceNameAsync(apiResourceId);

            return apiScopesDto;
        }

        public async Task<int> AddApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            return await ApiResourceRepository.AddApiScopeAsync(apiScope.ApiResourceId, scope);
        }

        public async Task<int> UpdateApiScopeAsync(ApiScopesDto apiScope)
        {
            var canInsert = await CanInsertApiScopeAsync(apiScope);
            if (!canInsert)
            {
                await BuildApiScopesViewModelAsync(apiScope);
                throw new UserFriendlyViewException(string.Format(ApiResourceServiceResources.ApiScopeExistsValue().Description, apiScope.Name), ApiResourceServiceResources.ApiScopeExistsKey().Description, apiScope);
            }

            var scope = apiScope.ToEntity();

            return await ApiResourceRepository.UpdateApiScopeAsync(apiScope.ApiResourceId, scope);
        }

        public async Task<int> DeleteApiScopeAsync(ApiScopesDto apiScope)
        {
            var scope = apiScope.ToEntity();

            return await ApiResourceRepository.DeleteApiScopeAsync(scope);
        }

        public async Task<ApiSecretsDto> GetApiSecretsAsync(int apiResourceId, int page = 1, int pageSize = 10)
        {
            var apiResource = await ApiResourceRepository.GetApiResourceAsync(apiResourceId);
            if (apiResource == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiResourceDoesNotExist().Description, apiResourceId), ApiResourceServiceResources.ApiResourceDoesNotExist().Description);

            var pagedList = await ApiResourceRepository.GetApiSecretsAsync(apiResourceId, page, pageSize);

            var apiSecretsDto = pagedList.ToModel();
            apiSecretsDto.ApiResourceId = apiResourceId;
            apiSecretsDto.ApiResourceName = await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);

            return apiSecretsDto;
        }

        public async Task<int> AddApiSecretAsync(ApiSecretsDto apiSecret)
        {
            HashApiSharedSecret(apiSecret);

            var secret = apiSecret.ToEntity();

            return await ApiResourceRepository.AddApiSecretAsync(apiSecret.ApiResourceId, secret);
        }

        public async Task<ApiSecretsDto> GetApiSecretAsync(int apiSecretId)
        {
            var apiSecret = await ApiResourceRepository.GetApiSecretAsync(apiSecretId);
            if (apiSecret == null) throw new UserFriendlyErrorPageException(string.Format(ApiResourceServiceResources.ApiSecretDoesNotExist().Description, apiSecretId), ApiResourceServiceResources.ApiSecretDoesNotExist().Description);
            var apiSecretsDto = apiSecret.ToModel();

            return apiSecretsDto;
        }

        public async Task<int> DeleteApiSecretAsync(ApiSecretsDto apiSecret)
        {
            var secret = apiSecret.ToEntity();

            return await ApiResourceRepository.DeleteApiSecretAsync(secret);
        }

        public async Task<bool> CanInsertApiScopeAsync(ApiScopesDto apiScopes)
        {
            var apiScope = apiScopes.ToEntity();

            return await ApiResourceRepository.CanInsertApiScopeAsync(apiScope);
        }

        public async Task<string> GetApiResourceNameAsync(int apiResourceId)
        {
            return await ApiResourceRepository.GetApiResourceNameAsync(apiResourceId);
        }
    }
}