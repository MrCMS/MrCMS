using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
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

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderPartial("Layout", webpage);
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