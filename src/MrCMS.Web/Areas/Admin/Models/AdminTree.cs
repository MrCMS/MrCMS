using System.Collections.Generic;

namespace MrCMS.Web.Areas.Admin.Models
{
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
