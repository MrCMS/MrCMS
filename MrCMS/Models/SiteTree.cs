using System.Web.Mvc;

namespace MrCMS.Models
{
    public class SiteTree<T> : SiteTreeNode<T>
    {
        public int? SiteId { get; set; }
    }
}