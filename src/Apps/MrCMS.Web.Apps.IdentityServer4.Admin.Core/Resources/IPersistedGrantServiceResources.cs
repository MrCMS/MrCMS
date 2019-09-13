

using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Helpers;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Resources
{
    public interface IPersistedGrantServiceResources
    {
        ResourceMessage PersistedGrantDoesNotExist();

        ResourceMessage PersistedGrantWithSubjectIdDoesNotExist();
    }
}
