using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MrCMS.Services;
using MrCMS.Website;

namespace MrCMS.Entities.Documents.Media
{
    [DocumentTypeDefinition(ChildrenListType.WhiteList, Name = "Media Category", IconClass = "icon-film", DisplayOrder = 1, Type = typeof(MediaCategory), WebGetAction = "View", WebGetController = "MediaCategory")]
    public class MediaCategory : Document
    {
        public virtual string MetaTitle { get; set; }
        public virtual string MetaDescription { get; set; }

        [DisplayName("Is Gallery")]
        public virtual bool IsGallery { get; set; }

        private IList<MediaFile> _files = new List<MediaFile>();

        public virtual IList<MediaFile> Files
        {
            get { return _files; }
            protected internal set { _files = value; }
        }

        public override void OnDeleting()
        {
            base.OnDeleting();
            
            var mediaFiles = Files.ToList();

            var fileService = MrCMSApplication.Get<IFileService>();
            foreach (var mediaFile in mediaFiles)
                fileService.DeleteFile(mediaFile);
            fileService.RemoveFolder(this);
        }
    }
}
