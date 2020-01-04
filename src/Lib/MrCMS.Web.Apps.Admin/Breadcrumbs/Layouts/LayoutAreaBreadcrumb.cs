using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Layouts
{
    public class LayoutAreaBreadcrumb : ItemBreadcrumb<LayoutAreasBreadcrumb, LayoutArea>
    {
        public LayoutAreaBreadcrumb(IRepository<LayoutArea> session) : base(session)
        {
        }
        public override async Task Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var item = await Repository.GetData(Id.Value);
            Name = item.AreaName;
            ParentActionArguments = CreateIdArguments(item.Layout?.Id);
        }
    }
}