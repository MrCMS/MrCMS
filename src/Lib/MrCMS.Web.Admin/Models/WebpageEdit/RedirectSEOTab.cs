using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Models.Tabs;

namespace MrCMS.Web.Admin.Models.WebpageEdit
{
    public class RedirectSEOTab : AdminTab<Webpage>
    {
        public override int Order => 100;

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "SEO & Properties";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return (entity is Redirect);
        }

        public override Type ParentType => null;

        public override Type ModelType => typeof(RedirectSEOTabViewModel);

        public override string TabHtmlId => "edit-seo";

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("RedirectSEO", mapper.Map<RedirectSEOTabViewModel>(webpage));
        }
    }
}