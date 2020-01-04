using System.ComponentModel.DataAnnotations;
using MrCMS.DbConfiguration.Configuration;

namespace MrCMS.Entities.Documents.Media
{
    public class ResizedImage : SiteEntity
    {
        public virtual MediaFile MediaFile { get; set; }
        public int MediaFileId { get; set; }
        public virtual Crop Crop { get; set; }
        [StringLength(450)]
        public virtual string Url { get; set; }

    }
}