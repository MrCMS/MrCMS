namespace MrCMS.Web.Areas.Admin.Models
{
    public class MoveWebpageChangedPageModel
    {
        public int Id { get; set; }
        public string OldHierarchy { get; set; }
        public string OldUrl { get; set; }
        public string NewHierarchy { get; set; }
        public string NewUrl { get; set; }
        public int? ParentId { get; set; }
    }
}