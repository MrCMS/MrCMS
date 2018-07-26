using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public abstract class WebpageTab : WebpageTabBase
    {
        public abstract string TabHtmlId { get; }
        public abstract Task RenderTabPane(IHtmlHelper<Webpage> html, Webpage webpage);
    }
}