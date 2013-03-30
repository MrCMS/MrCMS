using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Entities.Documents.Media
{
    public class MediaCategory : Document
    {
        [Required]
        [Remote("ValidateUrlIsAllowed", "MediaCategory", AdditionalFields = "Id")]
        [RegularExpression("[a-zA-Z0-9\\-\\.\\~\\/_\\\\]+$", ErrorMessage = "Path must alphanumeric characters only with dashes or underscore for spaces.")]
        [DisplayName("Path")]
        public override string UrlSegment { get; set; }

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

        public override void OnDeleting(ISession session)
        {
            base.OnDeleting(session);
            
            var mediaFiles = Files.ToList();

            var fileService = MrCMSApplication.Get<IFileService>();
            foreach (var mediaFile in mediaFiles)
                fileService.DeleteFile(mediaFile);
            fileService.RemoveFolder(this);
        }
    }
}
