using System.Web.Mvc;

namespace MrCMS.Website.Optimization
{
    public interface IResourceBundler
    {
        void AddScript(string virtualPath, string url);
        void AddCss(string virtualPath, string url);
        MvcHtmlString GetScripts();
        MvcHtmlString GetCss();
    }
}