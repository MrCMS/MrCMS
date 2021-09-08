using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface IGetHomePage
    {
        Task<Webpage> Get();
        Task<Webpage> GetForSite(Site site);
    }
}