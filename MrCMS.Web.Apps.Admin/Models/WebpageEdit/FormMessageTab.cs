using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
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

        public override Task RenderTabPane(IHtmlHelper<Webpage> html, Webpage webpage)
        {
            return html.RenderPartialAsync("FormMessage", webpage);
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