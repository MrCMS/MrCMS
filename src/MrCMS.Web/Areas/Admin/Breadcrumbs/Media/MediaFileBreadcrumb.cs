using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;

namespace MrCMS.Web.Areas.Admin.Breadcrumbs.Media
{
    public class MediaFileBreadcrumb : ItemBreadcrumb<MediaFolderBreadcrumb, MediaFile>
    {
        public MediaFileBreadcrumb(IRepository<MediaFile> repository) : base(repository)
        {
        }

        public override string Controller => "File";

        public override async Task Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var mediaFile = await Repository.GetData(Id.Value);
            Name = mediaFile.FileName;
            ParentActionArguments = CreateIdArguments(mediaFile.MediaCategory?.Id);
        }
    }
}