using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class FormPropertiesTab : WebpageTab
    {
        public override int Order
        {
            get { return 0; }
        }

        public override string Name(Webpage webpage)
        {
            return "Fields";
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
            get { return "form-properties-tab"; }
        }

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderPartial("FormProperties", webpage);
        }
    }
}