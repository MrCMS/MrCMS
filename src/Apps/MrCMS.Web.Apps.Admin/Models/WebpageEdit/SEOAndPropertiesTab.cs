using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;
using System;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class SEOAndPropertiesTab : AdminTab<Webpage>
    {
        public override int Order => 100;

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "SEO & Properties";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return !(entity is Redirect);
        }

        public override Type ParentType => null;

        public override Type ModelType => typeof(SEOTabViewModel);

        public override string TabHtmlId => "edit-seo";

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("SEO", mapper.Map<SEOTabViewModel>(webpage));
        }
    }
}