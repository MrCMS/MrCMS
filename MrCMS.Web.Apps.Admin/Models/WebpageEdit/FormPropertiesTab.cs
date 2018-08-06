using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class FormPropertiesTab : AdminTab<Webpage>
    {
        public override int Order => 0;

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "Fields";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return true;
        }

        public override Type ParentType => typeof(FormTab);

        public override Type ModelType => null;

        public override string TabHtmlId
        {
            get { return "form-properties-tab"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("FormProperties", webpage);
        }
    }
}