using MrCMS.Helpers;

namespace MrCMS.Models
{
    public class ImageSortItem : SortItem
    {
        public bool IsImage => MediaFileExtensions.IsImageExtension(FileExtension);
        public string FileExtension{ get; set; }
        public string ImageUrl { get; set; }
    }
}