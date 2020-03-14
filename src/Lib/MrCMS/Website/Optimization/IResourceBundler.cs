using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MrCMS.Website.Optimization
{
    public interface IResourceBundler
    {
        void AddScript(IHtmlHelper helper, string url);
        void AddCss(IHtmlHelper helper, string url);
        Task GetScripts(ViewContext viewContext);
        Task GetCss(ViewContext viewContext);
    }
}