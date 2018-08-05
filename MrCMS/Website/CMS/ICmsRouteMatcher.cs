using System.Threading.Tasks;

namespace MrCMS.Website.CMS
{
    public interface ICmsRouteMatcher
    {
        Task<CmsMatchData> TryMatch(string path, string method);
    }
}