using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Website.Optimization
{
    public interface IResourceBundler
    {
        void AddScript(IHtmlHelper helper, string url);
        void AddCss(IHtmlHelper helper, string url);
        void GetScripts(ViewContext viewContext);
        void GetCss(ViewContext viewContext);
    }
}