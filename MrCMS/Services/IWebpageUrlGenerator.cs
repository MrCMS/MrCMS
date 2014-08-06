using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    internal interface IWebpageUrlGenerator
    {
        string GetUrl(string pageName, Webpage parent, bool useHierarchy);
    }
}