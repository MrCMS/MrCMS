using MrCMS.Entities.Documents.Layout;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.Layouts
{
    public class LayoutAreaBreadcrumb : ItemBreadcrumb<LayoutAreasBreadcrumb, LayoutArea>
    {
        public LayoutAreaBreadcrumb(ISession session) : base(session)
        {
        }
        public override void Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var item = Session.Get<LayoutArea>(Id.Value);
            Name = item.AreaName;
            ParentActionArguments = CreateIdArguments(item.Layout?.Id);
        }
    }
}