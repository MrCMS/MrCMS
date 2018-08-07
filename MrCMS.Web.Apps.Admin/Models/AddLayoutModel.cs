namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddLayoutModel
    {
        public string Name { get; set; }
        public string UrlSegment { get; set; }
        public int? ParentId { get; set; }
    }
}