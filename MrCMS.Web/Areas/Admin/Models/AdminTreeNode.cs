namespace MrCMS.Web.Areas.Admin.Models
{
    public class AdminTreeNode
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public string Type { get; set; }
        public string NodeType { get; set; }
        public bool HasChildren { get; set; }
        public bool Sortable { get; set; }
        public bool CanAddChild { get; set; }
        public bool IsPublished { get; set; }
        public bool RevealInNavigation { get; set; }
        public bool IsMoreLink { get; set; }
        public int? NumberMore { get; set; }
        public string Url { get; set; }
    }
}