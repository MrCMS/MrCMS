using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class SEOAndPropertiesTab : WebpageTab
    {
        public override int Order
        {
            get { return 100; }
        }

        public override string Name(Webpage webpage)
        {
            return "SEO & Properties";
        }

        public override bool ShouldShow(Webpage webpage)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override string TabHtmlId
        {
            get { return "edit-seo"; }
        }

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderPartial("SEO", webpage);
        }
    }
}