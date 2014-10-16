using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class FormDesignTab : WebpageTab
    {
        public override int Order
        {
            get { return 100; }
        }

        public override string Name(Webpage webpage)
        {
            return "Design";
        }

        public override bool ShouldShow(Webpage webpage)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return typeof(FormTab); }
        }

        public override string TabHtmlId
        {
            get { return "form-design-tab"; }
        }

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderPartial("FormDesign", webpage);
        }
    }
}