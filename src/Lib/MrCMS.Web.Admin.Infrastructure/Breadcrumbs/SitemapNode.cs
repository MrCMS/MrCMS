using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Admin.Infrastructure.Breadcrumbs
{
    public class SitemapNode
    {
        public SitemapNode(Breadcrumb breadcrumb, string url)
        {
            BreadcrumbType = breadcrumb.GetType();
            Name = breadcrumb.Name;
            Order = breadcrumb.Order;
            CssClass = breadcrumb.CssClass;
            Url = url;
        }

        public Type BreadcrumbType { get; set; }
        public string Name { get; private set; }
        public string Url { get; }
        // public string Controller { get; }
        // public string Action { get; }
        public List<SitemapNode> Nodes { get; } = new List<SitemapNode>();
        public SitemapNode Parent { get; set; }

        public void AddChildren(List<SitemapNode> nodes)
        {
            foreach (var node in nodes)
            {
                node.Parent = this;
                Nodes.Add(node);
            }
        }

        public IEnumerable<SitemapNode> SelfAndChildren
        {
            get
            {
                yield return this;
                foreach (var node in AllChildren)
                {
                    yield return node;
                }
            }
        }
        public IEnumerable<SitemapNode> AllChildren
        {
            get
            {
                foreach (var node in Nodes)
                {
                    yield return node;
                    foreach (var child in node.AllChildren)
                    {
                        yield return child;
                    }
                }
            }
        }

        public int Order { get; }
        // public bool HasRouteInfo => !string.IsNullOrWhiteSpace(Controller) && !string.IsNullOrWhiteSpace(Action);
        public string CssClass { get; }

        public bool IsActive(List<PageHeaderBreadcrumb> breadcrumbs)
            => SelfAndChildren.Any(node => breadcrumbs.Any(x => x.BreadcrumbType == node.BreadcrumbType));

        public string SelectedClass(List<PageHeaderBreadcrumb> breadcrumbs, string cssClass = "active open")
        {
            return IsActive(breadcrumbs)
                ? cssClass
                : string.Empty;
        }
    }
}