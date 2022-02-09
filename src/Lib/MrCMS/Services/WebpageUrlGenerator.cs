using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public abstract class WebpageUrlGenerator<T> : IWebpageUrlGenerator where T : Webpage
    {
        public abstract string GetUrl(string pageName, Webpage parent, bool useHierarchy);
    }
}