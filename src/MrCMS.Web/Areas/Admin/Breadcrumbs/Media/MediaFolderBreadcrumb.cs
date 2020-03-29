using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Media
{
    public class MediaFolderBreadcrumb : ItemBreadcrumb<MediaBreadcrumb, MediaCategory>
    {
        public MediaFolderBreadcrumb(IRepository<MediaCategory> repository) : base(repository)
        {
        }

        public override string Controller => "MediaCategory";
        public override string Action => "Show";
        public override bool IsNav => false;
        public override bool Hierarchical => ParentId.HasValue;

        public override async Task Populate()
        {
            if (!Id.HasValue)
                return;
            var item = await Repository.GetData(Id.Value);
            Name = GetName(item);
            ParentActionArguments = CreateIdArguments(item.ParentId);
        }
    }
}