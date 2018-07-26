using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class PropertiesTab : WebpageTab
    {
        public override int Order
        {
            get { return 0; }
        }

        public override string Name(Webpage webpage)
        {
            return "Properties";
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
            get { return "edit-content"; }
        }

        public override Task RenderTabPane(IHtmlHelper<Webpage> html, Webpage webpage)
        {
            return html.RenderPartialAsync("Properties", webpage);
        }
    }
}