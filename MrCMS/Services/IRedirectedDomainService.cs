using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface IRedirectedDomainService
    {
        void Save(RedirectedDomain domain);
        void Delete(RedirectedDomain domain);
    }
}