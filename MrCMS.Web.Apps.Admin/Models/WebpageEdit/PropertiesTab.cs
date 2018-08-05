using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class PropertiesTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 0; }
        }

        public override string Name(IHtmlHelper helper, Webpage entity)
        {
            return "Properties";
        }

        public override bool ShouldShow(IHtmlHelper helper, Webpage entity)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override Type ModelType => typeof(PropertiesTabViewModel);

        public override string TabHtmlId
        {
            get { return "edit-content"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("Properties", mapper.Map<PropertiesTabViewModel>(webpage));
        }
    }
}