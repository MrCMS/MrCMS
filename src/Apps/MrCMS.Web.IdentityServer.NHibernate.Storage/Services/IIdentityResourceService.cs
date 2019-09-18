using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public interface IIdentityResourceService
    {
        void Add(IdentityResource resource);

        void Update(IdentityResource resource);
    }
}