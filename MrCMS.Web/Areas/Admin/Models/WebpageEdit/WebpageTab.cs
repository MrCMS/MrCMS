using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public abstract class WebpageTab : WebpageTabBase
    {
        public abstract string TabHtmlId { get; }
        public abstract void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage);
    }
}