using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Areas.Admin.Models.WebpageEdit
{
    public class PermissionsTab : WebpageTab
    {
        public override int Order
        {
            get { return 500; }
        }

        public override string Name(Webpage webpage)
        {
            return "Permissions";
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
            get { return "permissions"; }
        }

        public override void RenderTabPane(HtmlHelper<Webpage> html, Webpage webpage)
        {
            html.RenderPartial("Permissions", webpage);
        }
    }
}