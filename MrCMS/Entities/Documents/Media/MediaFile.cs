using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace MrCMS.Entities.Documents.Media
{
    public class MediaFile : SiteEntity
    {
        public MediaFile()
        {
            ResizedImages = new List<ResizedImage>();
        }

        public virtual string FileExtension { get; set; }
        public virtual string ContentType { get; set; }
        public virtual MediaCategory MediaCategory { get; set; }
        public virtual string FileUrl { get; set; }
        public virtual long ContentLength { get; set; }
        public virtual string FileName { get; set; }

        public virtual int DisplayOrder { get; set; }

        [DisplayName("Alt")]
        public virtual string Title { get; set; }

        public virtual string Description { get; set; }

        //images only
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }

        public virtual Size Size
        {
            get { return Width > 0 && Height > 0 ? new Size(Width, Height) : Size.Empty; }
        }

        public virtual IList<ResizedImage> ResizedImages { get; set; }
    }
}