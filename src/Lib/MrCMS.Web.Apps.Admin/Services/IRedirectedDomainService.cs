using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Apps.Admin.Services
{
    public interface IRedirectedDomainService
    {
        Task Save(RedirectedDomain domain);
        Task Delete(int id);
    }
}