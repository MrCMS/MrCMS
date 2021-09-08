using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IUrlValidationService
    {
        Task<bool> UrlIsValidForMediaCategory(string urlSegment, int? id);
        Task<bool> UrlIsValidForLayout(string urlSegment, int? id);
        Task<bool> UrlIsValidForWebpage(string url, int? id);
        Task<bool> UrlIsValidForWebpageUrlHistory(string url);
        Task<UrlHistory> Get(int id);
    }
}