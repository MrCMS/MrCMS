using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using X.PagedList;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Services
{
    public interface IIdentityResourceService
    {
        Task<IdentityResourcesDto> GetIdentityResourcesAsync(string search, int page = 1, int pageSize = 10);

        Task<IdentityResourceDto> GetIdentityResourceAsync(int identityResourceId);

        Task<bool> CanInsertIdentityResourceAsync(IdentityResourceDto identityResource);

        Task<int> AddIdentityResourceAsync(IdentityResourceDto identityResource);

        Task<int> UpdateIdentityResourceAsync(IdentityResourceDto identityResource);

        Task<int> DeleteIdentityResourceAsync(IdentityResourceDto identityResource);

        Task<int> DeleteIdentityResourceAsync(int identityResourceId);

        IdentityResourceDto BuildIdentityResourceViewModel(IdentityResourceDto identityResource);

        Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertiesAsync(int identityResourceId, int page = 1,
            int pageSize = 10);

        Task<IdentityResourcePropertiesDto> GetIdentityResourcePropertyAsync(int identityResourcePropertyId);

        Task<int> AddIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperties);

        Task<int> DeleteIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourceProperty);

        Task<int> DeleteIdentityResourcePropertyAsync(int identityResourcePropertyId);

        Task<bool> CanInsertIdentityResourcePropertyAsync(IdentityResourcePropertiesDto identityResourcePropertiesDto);
    }
}
