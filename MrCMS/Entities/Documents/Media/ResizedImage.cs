namespace MrCMS.Entities.Documents.Media
{
    public class ResizedImage : SiteEntity
    {
        public virtual MediaFile MediaFile { get; set; }
        public virtual Crop Crop { get; set; }
        public virtual string Url { get; set; }
    }
}