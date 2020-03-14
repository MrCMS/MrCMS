using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.Sitemaps
{
    public interface IReasonToExcludePageFromSitemap
    {
        Task<bool> ShouldExclude(Webpage webpage);
    }
}