using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Models.Tabs;

namespace MrCMS.Web.Areas.Admin.Models.Forms
{
    public class FormDesignTab : AdminTab<Form>
    {
        public override int Order
        {
            get { return 100; }
        }

        public override string Name(IServiceProvider serviceProvider, Form entity)
        {
            return "Design";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Form entity)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override Type ModelType => typeof(FormDesignTabViewModel);

        public override string TabHtmlId
        {
            get { return "form-design-tab"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Form webpage)
        {
            return html.RenderPartialAsync("FormDesign", mapper.Map<FormDesignTabViewModel>(webpage));
        }
    }
}