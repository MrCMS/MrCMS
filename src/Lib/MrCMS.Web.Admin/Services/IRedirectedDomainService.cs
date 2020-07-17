using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Admin.Services
{
    public interface IRedirectedDomainService
    {
        void Save(RedirectedDomain domain);
        void Delete(int id);
    }
}