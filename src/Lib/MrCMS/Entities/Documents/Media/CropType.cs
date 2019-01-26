using System.Drawing;

namespace MrCMS.Entities.Documents.Media
{
    public class CropType : SiteEntity
    {
        public virtual string Name { get; set; }
        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual Size Size
        {
            get { return new Size(Width, Height); }
        }
    }
}