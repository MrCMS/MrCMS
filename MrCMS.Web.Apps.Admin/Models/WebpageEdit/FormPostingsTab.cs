using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class FormPostingsTab : WebpageTab
    {
        public override int Order
        {
            get { return 200; }
        }

        public override string Name(Webpage webpage)
        {
            //return string.Format("Entries ({0})", webpage.FormPostingsCount());
            return "Entries"; // TODO: add count back in
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

        public override Task RenderTabPane(IHtmlHelper<Webpage> html, Webpage webpage)
        {
            return html.RenderPartialAsync("FormPostings", webpage);
        }
    }
}