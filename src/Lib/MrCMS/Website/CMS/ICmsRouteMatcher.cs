using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Website.CMS
{
    public interface ICmsRouteMatcher
    {
        CmsMatchData TryMatch(string path, string method);
        CmsMatchData Match(Webpage webpage, string method);
    }
}