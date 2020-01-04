using System.Threading.Tasks;
using MrCMS.Entities.Multisite;

namespace MrCMS.Services
{
    public interface ICurrentSiteLocator
    {
        Task<Site> GetCurrentSite();
    }
}