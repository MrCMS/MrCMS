using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.Layouts
{
    public class LayoutBreadcrumb : ItemBreadcrumb<LayoutsBreadcrumb, Layout>
    {
        public override decimal Order => 0.5m;

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
            ParentActionArguments = CreateIdArguments(item.Parent?.Id);
        }
    }
}