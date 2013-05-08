using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Settings;
using MrCMS.Website;

namespace MrCMS.Entities.Documents.Media
{
    public class MediaFile : SiteEntity
    {
        public virtual string FileExtension { get; set; }
        public virtual string ContentType { get; set; }
        public virtual MediaCategory MediaCategory { get; set; }
        //public virtual string FileLocation { get; set; }
        public virtual string FileUrl { get; set; }
        public virtual int ContentLength { get; set; }
        public virtual string FileName { get; set; }

        public virtual int DisplayOrder { get; set; }

        [DisplayName("Alt")]
        public virtual string Title { get; set; }
        [DisplayName("Title")]
        public virtual string Description { get; set; }

        //images only
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual Size Size { get { return Width > 0 && Height > 0 ? new Size(Width, Height) : Size.Empty; } }

        public static readonly List<string> ImageExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
        public virtual bool IsImage
        {
            get { return ImageExtensions.Any(s => s.Equals(FileExtension, StringComparison.InvariantCultureIgnoreCase)); }
        }

        public virtual IEnumerable<ImageSize> Sizes
        {
            get
            {
                if (IsImage)
                {
                    yield return new ImageSize("Original", Size);
                    foreach (
                        var imageSize in
                            MrCMSApplication.Get<MediaSettings>().ImageSizes.Where(size => ImageProcessor.RequiresResize(Size, size.Size)))
                    {
                        imageSize.ActualSize = ImageProcessor.CalculateDimensions(Size, imageSize.Size);
                        yield return imageSize;
                    }
                }
            }
        }
    }
}