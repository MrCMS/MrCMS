using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public interface IWebpageChildrenChecker
    {
        bool CanAddChildren(Webpage webpage);
    }
}