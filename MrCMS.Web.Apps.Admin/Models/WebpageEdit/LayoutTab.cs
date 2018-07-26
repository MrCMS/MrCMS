using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class LayoutTab : WebpageTab
    {
        public override int Order
        {
            get { return 200; }
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override string TabHtmlId
        {
            get { return "layout-content"; }
        }

        public override Task RenderTabPane(IHtmlHelper<Webpage> html, Webpage webpage)
        {
            return html.RenderPartialAsync("Layout", webpage);
        }

        public override string Name(Webpage webpage)
        {
            return "Layout";
        }

        public override bool ShouldShow(Webpage webpage)
        {
            return true;
        }
    }
}