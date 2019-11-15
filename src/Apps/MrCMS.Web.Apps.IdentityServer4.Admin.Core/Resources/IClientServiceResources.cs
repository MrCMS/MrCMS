

using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Helpers;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Resources
{
    public interface IClientServiceResources
    {
        ResourceMessage ClientClaimDoesNotExist();

        ResourceMessage ClientDoesNotExist();

        ResourceMessage ClientExistsKey();

        ResourceMessage ClientExistsValue();

        ResourceMessage ClientPropertyDoesNotExist();

        ResourceMessage ClientSecretDoesNotExist();
    }
}
