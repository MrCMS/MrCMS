using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class SEOAndPropertiesTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 100; }
        }

        public override string Name(IHtmlHelper helper, Webpage entity)
        {
            return "SEO & Properties";
        }

        public override bool ShouldShow(IHtmlHelper helper, Webpage entity)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override Type ModelType => typeof(SEOTabViewModel);

        public override string TabHtmlId
        {
            get { return "edit-seo"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("SEO", mapper.Map<SEOTabViewModel>(webpage));
        }
    }
}