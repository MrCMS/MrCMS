using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
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

        public override Task RenderTabPane(IHtmlHelper<Webpage> html, Webpage webpage)
        {
            return html.RenderPartialAsync("Permissions", webpage);
        }
    }
}