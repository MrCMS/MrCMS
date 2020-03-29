using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Layouts
{
    public class LayoutBreadcrumb : ItemBreadcrumb<LayoutsBreadcrumb, Layout>
    {
        public LayoutBreadcrumb(IRepository<Layout> repository) : base(repository)
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