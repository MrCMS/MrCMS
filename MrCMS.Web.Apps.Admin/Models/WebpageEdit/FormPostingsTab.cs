using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class FormPostingsTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 200; }
        }

        public override string Name(IHtmlHelper helper, Webpage entity)
        {
            //return string.Format("Entries ({0})", webpage.FormPostingsCount());
            return "Entries"; // TODO: add count back in
        }

        public override bool ShouldShow(IHtmlHelper helper, Webpage entity)
        {
            return true;
        }

        public override Type ParentType => typeof(FormTab);

        public override Type ModelType => null;

        public override string TabHtmlId => "form-postings-tab";

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("FormPostings", webpage);
        }
    }
}