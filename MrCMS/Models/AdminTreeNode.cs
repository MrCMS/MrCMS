using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MrCMS.Models
{
    public class AdminTreeNode
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public string NodeType { get; set; }
        public bool HasChildren { get; set; }
        public bool Sortable { get; set; }
        public bool CanAddChild { get; set; }
        public bool IsPublished { get; set; }
        public bool RevealInNavigation { get; set; }
        public bool IsMoreLink { get; set; }
        public int? NumberMore { get; set; }
    }

    public class AdminTree
    {
        public AdminTree()
        {
            Nodes = new List<AdminTreeNode>();
        }
        public List<AdminTreeNode> Nodes { get; set; }
        public string RootContoller { get; set; }
        public bool IsRootRequest { get; set; }
    }
}
