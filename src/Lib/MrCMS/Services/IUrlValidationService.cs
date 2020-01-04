using System.Threading.Tasks;

namespace MrCMS.Services
{
    public interface IUrlValidationService
    {
        Task<bool> UrlIsValidForMediaCategory(string urlSegment, int? id);
        Task<bool> UrlIsValidForLayout(string urlSegment, int? id);
        Task<bool> UrlIsValidForWebpage(string url, int? id);
        Task<bool> UrlIsValidForWebpageUrlHistory(string url);
    }
}