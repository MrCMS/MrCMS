using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class VersionsTab : WebpageTab
    {
        public override int Order
        {
            get { return 400; }
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override string TabHtmlId
        {
            get { return "versions"; }
        }

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderAction("Show", "Versions", new {document = webpage});
        }

        public override string Name(Webpage webpage)
        {
            return "Versions";
        }

        public override bool ShouldShow(Webpage webpage)
        {
            return true;
        }
    }
}