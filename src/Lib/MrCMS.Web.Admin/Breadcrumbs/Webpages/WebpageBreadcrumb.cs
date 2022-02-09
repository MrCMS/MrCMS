using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Admin.Breadcrumbs.Webpages
{
    public class WebpageBreadcrumb : ItemBreadcrumb<WebpagesBreadcrumb, Webpage>
    {
        public override decimal Order => -1;

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