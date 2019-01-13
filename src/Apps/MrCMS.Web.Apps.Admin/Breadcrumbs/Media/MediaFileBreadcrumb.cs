using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using NHibernate;

namespace MrCMS.Web.Apps.Admin.Breadcrumbs.Media
{
    public class MediaFileBreadcrumb : ItemBreadcrumb<MediaFolderBreadcrumb, MediaFile>
    {
        public MediaFileBreadcrumb(ISession session) : base(session)
        {
        }

        public override string Controller => "File";

        public override void Populate()
        {
            if (!Id.HasValue)
            {
                return;
            }

            var mediaFile = Session.Get<MediaFile>(Id.Value);
            Name = mediaFile.FileName;
            ParentActionArguments = CreateIdArguments(mediaFile.MediaCategory?.Id);
        }
    }
}