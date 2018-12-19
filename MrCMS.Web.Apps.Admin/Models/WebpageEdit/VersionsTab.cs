using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Models.WebpageEdit
{
    public class VersionsTab : AdminTab<Webpage>
    {
        public override int Order
        {
            get { return 400; }
        }

        public override Type ParentType
        {
            get { return null; }
        }

        public override Type ModelType => null;

        public override string TabHtmlId
        {
            get { return "versions"; }
        }

        public override async Task RenderTabPane(IHtmlHelper html, IMapper mapper, Webpage webpage)
        {
            await html.RenderPartialAsync("Versions", webpage);
        }

        public override string Name(IServiceProvider serviceProvider, Webpage entity)
        {
            return "Versions";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Webpage entity)
        {
            return true;
        }
    }
}