namespace MrCMS.Web.Apps.Admin.Models
{
    public class AddWebpageModel
    {
        public string DocumentType { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string UrlSegment { get; set; }
    }
}