using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUrlValidationService
    {
        Task<bool> UrlIsValidForMediaCategory(int siteId, string urlSegment, int? id);
        Task<bool> UrlIsValidForLayout(int siteId, string urlSegment, int? id);
        Task<bool> UrlIsValidForWebpage(int siteId, string url, int? id);
        Task<bool> UrlIsValidForWebpageUrlHistory(int siteId, string url);
        Task<UrlHistory> Get(int id);
    }
}