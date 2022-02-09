using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Web.Admin.Services
{
    public interface IRedirectedDomainService
    {
        Task Save(RedirectedDomain domain);
        Task Delete(int id);
    }
}