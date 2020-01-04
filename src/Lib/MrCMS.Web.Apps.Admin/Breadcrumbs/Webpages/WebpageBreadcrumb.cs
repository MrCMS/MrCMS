using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Webpages
{
    public class WebpageBreadcrumb : ItemBreadcrumb<WebpagesBreadcrumb, Webpage>
    {
        public WebpageBreadcrumb(IRepository<Webpage> repository) : base(repository)
        {
        }

        public override bool Hierarchical => ParentId.HasValue;
        public override async Task Populate()
        {
            if (!Id.HasValue)
                return;
            var item = await Repository.GetData(Id.Value);
            Name = GetName(item);
            ParentActionArguments = CreateIdArguments(item.Parent?.Id);
        }
    }
}