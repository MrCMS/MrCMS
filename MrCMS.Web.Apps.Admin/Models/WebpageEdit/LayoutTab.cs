using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class LayoutTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 200; }
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override Type ModelType => typeof(LayoutTabViewModel);

        public override string TabHtmlId => "layout-content";

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("Layout", mapper.Map<LayoutTabViewModel>(webpage));
        }

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "Layout";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return true;
        }
    }
}