using MrCMS.Entities.Widget;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.Layouts
{
    public class WidgetBreadcrumb : ItemBreadcrumb<WidgetsBreadcrumb, Widget>
    {
        public WidgetBreadcrumb(ISession session) : base(session)
        {
        }
        public override void Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var item = Session.Get<Widget>(Id.Value);
            Name = string.IsNullOrWhiteSpace(item.Name) ? $"{item.Name} ({item.WidgetTypeFormatted})" : item.WidgetTypeFormatted;
            ParentActionArguments = CreateIdArguments(item.LayoutArea?.Id);
        }
    }
}