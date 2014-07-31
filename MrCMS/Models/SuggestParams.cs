namespace MrCMS.Models
{
    public class SuggestParams
    {
        public string PageName { get; set; }

        public string DocumentType { get; set; }

        public int? Template { get; set; }

        public bool UseHierarchy { get; set; }
    }
}