using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Areas.Admin.Services
{
    public interface IRedirectedDomainService
    {
        void Save(RedirectedDomain domain);
        void Delete(RedirectedDomain domain);
    }
}