using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface ICmsRouteMatcher
    {
        Task<CmsMatchData> TryMatch(string path, string method);
        Task<CmsMatchData> Match(Webpage webpage, string method);
    }
}