using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services.Sitemaps
{
    public interface IReasonToExcludePageFromSitemap
    {
        bool ShouldExclude(Webpage webpage);
    }
}