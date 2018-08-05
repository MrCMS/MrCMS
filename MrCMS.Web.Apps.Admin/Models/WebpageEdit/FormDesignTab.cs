using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class FormDesignTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 100; }
        }

        public override string Name(IHtmlHelper helper, Webpage entity)
        {
            return "Design";
        }

        public override bool ShouldShow(IHtmlHelper helper, Webpage entity)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return typeof(FormTab); }
        }

        public override Type ModelType => typeof(FormDesignTabViewModel);

        public override string TabHtmlId
        {
            get { return "form-design-tab"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("FormDesign", mapper.Map<FormDesignTabViewModel>(webpage));
        }
    }
}