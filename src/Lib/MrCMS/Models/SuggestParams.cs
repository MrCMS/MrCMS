namespace MrCMS.Models
{
    public class SuggestParams
    {
        public int? ParentId { get; set; }
        public int? WebpageId { get; set; }

        public string PageName { get; set; }

        public string DocumentType { get; set; }

        public int? Template { get; set; }

        public bool UseHierarchy { get; set; }
    }
}