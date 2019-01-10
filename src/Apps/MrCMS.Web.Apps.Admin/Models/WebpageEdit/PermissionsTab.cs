using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;
using System;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class PermissionsTab : AdminTab<Webpage>
    {
        public override int Order => 500;

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "Permissions";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return !(entity is Redirect);
        }

        public override Type ParentType => null;

        public override Type ModelType => typeof(PermissionsTabViewModel);

        public override string TabHtmlId => "permissions";

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            return html.RenderPartialAsync("Permissions", mapper.Map<PermissionsTabViewModel>(webpage));
        }
    }
}