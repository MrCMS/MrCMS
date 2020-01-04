using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IWebpageUrlService
    {
        Task<string> Suggest(SuggestParams suggestParams);
    }
}