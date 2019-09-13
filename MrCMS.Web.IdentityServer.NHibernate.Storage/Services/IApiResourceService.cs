using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Services
{
    public interface IApiResourceService
    {
        void Add(ApiResource resource);

        void Update(ApiResource resource);
    }
}