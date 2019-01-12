using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs
{
    public class LayoutBreadcrumb : ItemBreadcrumb<LayoutsBreadcrumb, Layout>
    {
        public LayoutBreadcrumb(ISession session) : base(session)
        {
        }

        public override bool Hierarchical => ParentId.HasValue;
        public override void Populate()
        {
            if (!Id.HasValue)
                return;
            var item = Session.Get<Layout>(Id.Value);
            Name = GetName(item);
            ParentId = item.Parent?.Id;
        }
    }
}