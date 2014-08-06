namespace MrCMS.Web.Areas.Admin.Models
{
    public class MediaSelectorSearchQuery
    {
        public MediaSelectorSearchQuery()
        {
            Page = 1;
        }
        public int Page { get; set; }
        public int? CategoryId { get; set; }

        public string Query { get; set; }
    }
}