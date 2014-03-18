using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class ChildNodeHelper
    {
        public static HtmlNode GetElement(this HtmlNode node, string nodeType)
        {
            return node.ChildNodesRecursive().FirstOrDefault(htmlNode => htmlNode.Name == nodeType);
        }

        public static string GetElementText(this HtmlNode node, string nodeType)
        {
            var thisNode = node.ChildNodesRecursive().FirstOrDefault(htmlNode => htmlNode.Name == nodeType);

            return thisNode != null ? thisNode.InnerText : null;
        }

        public static string GetAllText(this HtmlNode node)
        {
            var text = node.ChildNodesRecursive().Select(htmlNode => htmlNode.InnerText).ToList();

            return string.Join(" ", text.Where(s => !string.IsNullOrWhiteSpace(s)));
        }

        public static IEnumerable<HtmlNode> GetElementsOfType(this HtmlNode node, string nodeType)
        {
            return node.ChildNodesRecursive().Where(htmlNode => htmlNode.Name == nodeType);
        }

        public static List<HtmlNode> ChildNodesRecursive(this HtmlNode node)
        {
            var nodes = new List<HtmlNode>();

            AddNodes(node, nodes);

            return nodes;
        }

        private static void AddNodes(HtmlNode node, List<HtmlNode> nodes)
        {
            var htmlNodeCollection = node.ChildNodes;
            nodes.AddRange(htmlNodeCollection);

            foreach (var childNode in htmlNodeCollection)
            {
                AddNodes(childNode, nodes);
            }
        }
    }
}