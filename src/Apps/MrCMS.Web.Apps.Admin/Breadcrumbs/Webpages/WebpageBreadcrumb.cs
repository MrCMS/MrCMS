using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Webpages
{
    public class WebpageBreadcrumb : ItemBreadcrumb<WebpagesBreadcrumb, Webpage>
    {
        public WebpageBreadcrumb(ISession session) : base(session)
        {
        }

        public override bool Hierarchical => ParentId.HasValue;
        public override void Populate()
        {
            if (!Id.HasValue)
                return;
            var item = Session.Get<Webpage>(Id.Value);
            Name = GetName(item);
            ParentActionArguments = CreateIdArguments(item.Parent?.Id);
        }
    }
}