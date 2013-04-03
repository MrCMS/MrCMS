namespace MrCMS.Models
{
    public class SortItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Order { get; set; }
    }

    public class ImageSortItem : SortItem
    {
        public bool IsImage { get; set; }
        public string ImageUrl { get; set; }
    }
}