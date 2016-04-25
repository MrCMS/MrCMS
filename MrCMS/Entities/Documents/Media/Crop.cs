using System;
using System.Collections.Generic;
using System.Drawing;

namespace MrCMS.Entities.Documents.Media
{
    public class Crop : SiteEntity
    {
        public Crop()
        {
            ResizedImages = new List<ResizedImage>();
        }

        public virtual MediaFile MediaFile { get; set; }
        public virtual CropType CropType { get; set; }
        public virtual int Left { get; set; }
        public virtual int Top { get; set; }
        public virtual int Right { get; set; }
        public virtual int Bottom { get; set; }
        public virtual string Url { get; set; }

        public virtual Size Size
        {
            get { return new Size(Width, Height); }
        }

        public virtual int Width
        {
            get { return Math.Max(Right - Left, 0); }
        }

        public virtual int Height
        {
            get { return Math.Max(Bottom - Top, 0); }
        }

        public virtual string Title
        {
            get
            {
                if (MediaFile != null) return MediaFile.Title;
                return string.Empty;
            }
        }

        public virtual string Description
        {
            get
            {
                if (MediaFile != null) return MediaFile.Description;
                return string.Empty;
            }
        }

        public virtual string FileExtension
        {
            get
            {
                if (MediaFile != null) return MediaFile.FileExtension;
                return string.Empty;
            }
        }

        public virtual IList<ResizedImage> ResizedImages { get; set; }

        public virtual string ContentType
        {
            get
            {
                if (MediaFile != null) return MediaFile.FileExtension;
                return string.Empty;
            }
        }
    }
}