using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Media
{
    public class MediaFolderBreadcrumb : ItemBreadcrumb<MediaBreadcrumb, MediaCategory>
    {
        public MediaFolderBreadcrumb(ISession session) : base(session)
        {
        }

        public override string Controller => "MediaCategory";
        public override string Action => "Show";
        public override bool IsNav => false;
        public override bool Hierarchical => ParentId.HasValue;

        public override void Populate()
        {
            if (!Id.HasValue)
                return;
            var item = Session.Get<MediaCategory>(Id.Value);
            Name = GetName(item);
            ParentActionArguments = CreateIdArguments(item.Parent?.Id);
        }
    }
}