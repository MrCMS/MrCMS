using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class FormMessageTab : WebpageTab
    {
        public override int Order
        {
            get { return 300; }
        }

        public override Type ParentType
        {
            get { return typeof(FormTab); }
        }

        public override string TabHtmlId
        {
            get { return "form-message-tab"; }
        }

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderPartial("FormMessage", webpage);
        }

        public override string Name(Webpage webpage)
        {
            return "Settings";
        }

        public override bool ShouldShow(Webpage webpage)
        {
            return true;
        }
    }
}