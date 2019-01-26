using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IRedirectedDomainService
    {
        void Save(RedirectedDomain domain);
        void Delete(RedirectedDomain domain);
    }
}