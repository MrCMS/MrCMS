using MrCMS.Entities.Documents.Web;
using MrCMS.Models;

namespace MrCMS.Services
{
    public interface IWebpageUrlService
    {
        string Suggest(SuggestParams suggestParams);
    }
}