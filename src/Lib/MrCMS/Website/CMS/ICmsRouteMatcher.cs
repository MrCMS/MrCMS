using System.Threading.Tasks;

namespace MrCMS.Website.CMS
{
    public interface ICmsRouteMatcher
    {
        CmsMatchData TryMatch(string path, string method);
    }
}