using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Models
{
    public class SiteTreeNode
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string IconClass { get; set; }
        public string NodeType { get; set; }
        public List<SiteTreeNode> Children { get; set; }
        public SiteTreeNode Parent { get; set; }
        public ChildModel ChildrenModel { get { return new ChildModel(Children, NumberMore, Id); } }

        public class ChildModel : List<SiteTreeNode>
        {
            public bool More { get { return NumberMore > 0; } }

            public int NumberMore { get; set; }

            public int? Id { get; set; }

            public ChildModel(IEnumerable<SiteTreeNode> children, int numberMore, int? id)
            {
                NumberMore = numberMore;
                Id = id;
                AddRange(children);
            }
        }

        public bool Sortable { get; set; }
        public bool CanAddChild { get; set; }
        public bool IsPublished { get; set; }
        public bool RevealInNavigation { get; set; }
        public int Total { get; set; }

        public int NumberMore
        {
            get { return Total - Children.Count; }
        }

        public List<SiteTreeNode> PublishedChildren { get { return Children.FindAll(node => node.IsPublished); } }
    }

    public class SiteTreeNode<T> : SiteTreeNode
    {
        public T Item { get; set; }
        public new List<SiteTreeNode<T>> Children { get { return base.Children.OfType<SiteTreeNode<T>>().ToList(); } set { base.Children = value.Cast<SiteTreeNode>().ToList(); } }
        public new List<SiteTreeNode<T>> PublishedChildren { get { return Children.FindAll(node => node.IsPublished); } }
        public List<SiteTreeNode> BaseChildren
        {
            get { return Children.Cast<SiteTreeNode>().ToList(); }
        }
    }
}