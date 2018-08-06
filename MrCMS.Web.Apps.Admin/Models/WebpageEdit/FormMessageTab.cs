using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class FormMessageTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 300; }
        }

        public override Type ParentType
        {
            get { return typeof(FormTab); }
        }

        public override Type ModelType => typeof(FormMessageTabViewModel);

        public override string TabHtmlId
        {
            get { return "form-message-tab"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("FormMessage", mapper.Map<FormMessageTabViewModel>(webpage));
        }

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "Settings";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return true;
        }
    }
}