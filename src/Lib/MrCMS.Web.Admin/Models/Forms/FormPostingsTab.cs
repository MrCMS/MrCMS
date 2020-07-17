using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Services;
using MrCMS.Web.Admin.Infrastructure.Models.Tabs;

namespace MrCMS.Web.Admin.Models.Forms
{
    public class FormPostingsTab : AdminTab<Form>
    {
        public override int Order => 200;

        public override string Name(IServiceProvider serviceProvider, Form entity)
        {
            var postingsModel = serviceProvider.GetRequiredService<IFormAdminService>()
                .GetFormPostings(entity, 1, string.Empty);
            return $"Entries ({postingsModel.Items.Count})";
        }

        public override bool ShouldShow(IServiceProvider serviceProvider, Form entity)
        {
            return true;
        }

        public override Type ParentType => null;

        public override Type ModelType => null;

        public override string TabHtmlId => "form-postings-tab";

        public override Task RenderTabPane(IHtmlHelper html, IMapper mapper, Form form)
        {
            return html.RenderPartialAsync("FormPostings", form);
        }
    }
}