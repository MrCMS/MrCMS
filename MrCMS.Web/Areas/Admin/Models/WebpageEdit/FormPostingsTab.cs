using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class FormPostingsTab : WebpageTab
    {
        public override int Order
        {
            get { return 200; }
        }

        public override string Name(Webpage webpage)
        {
            return string.Format("Entries ({0})", webpage.FormPostingsCount());
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
            get { return "form-postings-tab"; }
        }

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderPartial("FormPostings", webpage);
        }
    }
}