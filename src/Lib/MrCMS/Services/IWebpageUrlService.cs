using System.Threading.Tasks;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IWebpageUrlService
    {
        Task<string> Suggest(int siteId, SuggestParams suggestParams);
    }
}