using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.People;
using MrCMS.Web.Admin.Infrastructure.Models.Tabs;
using System;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Articles.Areas.Admin.Models
{
    public class AuthorInfoUserTab : AdminTab<User>
    {
        public override int Order => 1;

        public override Type ParentType => null;

        public override Type ModelType { get; }
        public override Task<string> Name(IServiceProvider serviceProvider, User entity)
        {
            return Task.FromResult("Author Info");
        }

        public override Task<bool> ShouldShow(IServiceProvider serviceProvider, User entity)
        {
            return Task.FromResult(true);
        }

        public override string TabHtmlId => "author-info";

        public override async Task RenderTabPane(IHtmlHelper helper, IMapper mapper, User entity)
        {
            await helper.RenderPartialAsync("~/Areas/Admin/Views/AuthorInfo/Show.cshtml", entity);
        }

    }
}
