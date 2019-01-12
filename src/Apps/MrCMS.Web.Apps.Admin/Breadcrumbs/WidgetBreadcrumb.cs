using MrCMS.Entities.Widget;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
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
            Name = string.IsNullOrWhiteSpace(item.Name) ? string.Format("{0} ({1})", item.Name, item.WidgetTypeFormatted) : item.WidgetTypeFormatted;
            ParentId = item.LayoutArea?.Id;
        }
    }
}