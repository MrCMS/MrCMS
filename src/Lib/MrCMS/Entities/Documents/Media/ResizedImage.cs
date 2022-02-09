using System.ComponentModel.DataAnnotations;

namespace MrCMS.Entities.Documents.Media
{
    public class ResizedImage : SiteEntity
    {
        public virtual MediaFile MediaFile { get; set; }
        public virtual Crop Crop { get; set; }
        [StringLength(450)] public virtual string Url { get; set; }
        public virtual bool Missing { get; set; }
        public virtual bool Cleansed { get; set; }
    }
}