using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
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

        public override Task RenderTabPane(IHtmlHelper<Webpage> html, Webpage webpage)
        {
            // TODO: render versions
            return Task.CompletedTask;
            //html.RenderPartialAsync("Show", "Versions", new {document = webpage});
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