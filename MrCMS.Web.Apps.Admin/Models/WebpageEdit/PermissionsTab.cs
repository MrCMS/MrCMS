using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class PermissionsTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 500; }
        }

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "Permissions";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return true;
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override Type ModelType => typeof(PermissionsTabViewModel);

        public override string TabHtmlId
        {
            get { return "permissions"; }
        }

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("Permissions", mapper.Map<PermissionsTabViewModel>(webpage));
        }
    }
}