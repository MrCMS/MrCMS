using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Layouts
{
    public class WidgetBreadcrumb : ItemBreadcrumb<WidgetsBreadcrumb, Widget>
    {
        public WidgetBreadcrumb(IRepository<Widget> session) : base(session)
        {
        }
        public override async Task Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var item = await Repository.GetData(Id.Value);
            Name = string.IsNullOrWhiteSpace(item.Name) ? $"{item.Name} ({item.WidgetTypeFormatted})" : item.WidgetTypeFormatted;
            ParentActionArguments = CreateIdArguments(item.LayoutArea?.Id);
        }
    }
}