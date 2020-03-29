using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Models.Tabs;

namespace MrCMS.Web.Areas.Admin.Models.Forms
{
    public class FormPropertiesTab : AdminTab<Form>
    {
        public override int Order => 0;

        public override string Name(IServiceProvider serviceProvider, Form entity)
        {
            return "Fields";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Form entity)
        {
            return true;
        }

        public override Type ParentType => null;

        public override Type ModelType => null;

        public override string TabHtmlId
        {
            get { return "form-properties-tab"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Form form)
        {
            return html.RenderPartialAsync("FormProperties", form);
        }
    }
}