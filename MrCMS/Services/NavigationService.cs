using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Website;
using NHibernate;
using Document = MrCMS.Entities.Documents.Document;

namespace MrCMS.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IDocumentService _documentService;
        private readonly ISession _session;
        private readonly Site _site;

        public NavigationService(IDocumentService documentService, ISession session, Site site)
        {
            _documentService = documentService;
            _session = session;
            _site = site;
        }

        public SiteTree<Webpage> GetWebsiteTree(int? depth = null)
        {
            var tree = new SiteTree<Webpage>
            {

                Id = null,
                ParentId = null,
                IconClass = "icon-asterisk",
                Name = "Root",
                NodeType = "Webpage",
                CanAddChild = DocumentMetadataHelper.GetValidWebpageDocumentTypes(null).Any(),
            };
            tree.Children = GetNodes(tree,
                                     _session.QueryOver<Webpage>()
                                             .Where(webpage => webpage.Parent == null && webpage.Site.Id == _site.Id)
                                             .OrderBy(webpage => webpage.DisplayOrder).Asc
                                             .Cacheable()
                                             .List(), depth);

            return tree;
        }

        public SiteTree<MediaCategory> GetMediaTree()
        {
            var tree = new SiteTree<MediaCategory>
            {
                Id = null,
                ParentId = null,
                IconClass = "icon-asterisk",
                Name = "Root",
                NodeType = "MediaCategory",
                CanAddChild = true,
            };
            tree.Children = GetNodes(tree,
                                     _session.QueryOver<MediaCategory>()
                                             .Where(category => category.Parent == null && category.Site.Id == _site.Id)
                                             .OrderBy(webpage => webpage.DisplayOrder).Asc
                                             .Cacheable()
                                             .List(), int.MaxValue);

            return tree;
        }

        public SiteTree<Layout> GetLayoutList()
        {
            var tree = new SiteTree<Layout>
            {
                Id = null,
                ParentId = null,
                IconClass = "icon-asterisk",
                Name = "Root",
                NodeType = "Layout",
                CanAddChild = true,
            };

            tree.Children = GetNodes(tree,
                                     _session.QueryOver<Layout>()
                                             .Where(layout => layout.Parent == null && layout.Site.Id == _site.Id)
                                             .OrderBy(webpage => webpage.DisplayOrder).Asc
                                             .Cacheable()
                                             .List(), int.MaxValue);
            return tree;
        }

        public IEnumerable<SelectListItem> GetParentsList()
        {
            var selectListItems = GetPageListItems(GetWebsiteTree().Children, 2).ToList();
            selectListItems.Insert(0, new SelectListItem { Selected = false, Text = "Root", Value = "0" });
            return selectListItems;
        }

        public string GetSiteMap(UrlHelper urlHelper)
        {
            var websiteTree = GetWebsiteTree();

            var xmlDocument = new XmlDocument();
            var xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.DocumentElement);

            var urlset =
                xmlDocument.AppendChild(xmlDocument.CreateElement("urlset"));


            var standardNs = xmlDocument.CreateAttribute("xmlns");
            standardNs.Value = "http://www.google.com/schemas/sitemap/0.9";
            urlset.Attributes.Append(standardNs);
            var imageNs = xmlDocument.CreateAttribute("xmlns:image");
            imageNs.Value = "http://www.google.com/schemas/sitemap-image/1.1";
            urlset.Attributes.Append(imageNs);

            var children = websiteTree.Children;

            while (children.Any())
            {
                children.ForEach(node =>
                {
                    var document = _documentService.GetDocument<Webpage>(node.Id.GetValueOrDefault());
                    if (document != null && document.Published)
                    {
                        var url =
                            urlset.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "url",
                                                                      ""));

                        var loc =
                            url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "loc", ""));
                        loc.InnerText = urlHelper.AbsoluteContent(document.LiveUrlSegment);
                        document.AddCustomSitemapData(urlHelper, url, xmlDocument);
                        var lastMod =
                            url.AppendChild(xmlDocument.CreateNode(XmlNodeType.Element, "lastmod",
                                                                   ""));

                        lastMod.InnerText = document.UpdatedOn.ToString("yyyy-MM-dd");

                    }
                });

                children = children.SelectMany(node => node.Children).ToList();
            }
            StringBuilder sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            xmlDocument.WriteTo(new XmlTextWriter(tw));

            var content = sb.ToString();
            return content;
        }

        public IEnumerable<SelectListItem> GetDocumentTypes(string type)
        {
            return DocumentMetadataHelper.DocumentMetadatas
                                   .BuildSelectItemList(definition => definition.Name, definition => definition.TypeName,
                                                        definition => definition.TypeName == type, "Select type");
        }

        public IEnumerable<IAdminMenuItem> GetNavLinks()
        {
            return TypeHelper.GetAllConcreteTypesAssignableFrom<IAdminMenuItem>()
                             .Select(MrCMSApplication.Get).Cast<IAdminMenuItem>()
                             .OrderBy(item => item.DisplayOrder);
        }

        private IEnumerable<SelectListItem> GetPageListItems(IEnumerable<SiteTreeNode<Webpage>> nodes, int depth)
        {
            var items = new List<SelectListItem>();

            foreach (var node in nodes.Where(node => node.Children.Any()))
            {
                items.Add(new SelectListItem { Selected = false, Text = GetDashes(depth) + node.Name, Value = node.Id.ToString() });
                items.AddRange(GetPageListItems(node.Children, depth + 1));
            }

            return items;
        }

        private string GetDashes(int depth)
        {
            return string.Empty.PadRight(depth * 2, '-');
        }


        private List<SiteTreeNode<T>> GetNodes<T>(SiteTreeNode parent, IEnumerable<T> docs, int? maxDepth = null) where T : Document
        {
            var list = new List<SiteTreeNode<T>>();
            docs.ForEach(doc =>
            {
                if (doc.ShowInAdminNav)
                {
                    var documentMetadata = doc.GetMetadata();
                    var queryOver = _session.QueryOver<T>().Where(arg => arg.Parent.Id == doc.Id);

                    queryOver = queryOver.OrderBy(webpage => webpage.DisplayOrder).Asc;

                    var nodeType = GetNodeType(doc);
                    var isWebpage = doc is Webpage;
                    var siteTreeNode = new SiteTreeNode<T>
                    {
                        Total = queryOver.Cacheable().RowCount(),
                        IconClass = documentMetadata == null ? "icon-file" : documentMetadata.IconClass,
                        Id = doc.Id,
                        ParentId = doc.Parent != null ? doc.Parent.Id : (int?)null,
                        Name = doc.Name,
                        NodeType = nodeType,
                        Sortable = IsSortable(doc),
                        CanAddChild = !isWebpage || (doc as Webpage).GetValidWebpageDocumentTypes().Any(),
                        IsPublished = !isWebpage || (doc as Webpage).Published,
                        RevealInNavigation = !isWebpage || (doc as Webpage).RevealInNavigation
                    };
                    if (ShowChildrenInAdminNav(doc))
                    {
                        var pagedList = queryOver.Paged(1, doc.GetMaxChildNodes() ?? int.MaxValue);
                        if (maxDepth.HasValue)
                            siteTreeNode.Children = maxDepth.Value == 0
                                                        ? new List<SiteTreeNode<T>>()
                                                        : GetNodes<T>(siteTreeNode, pagedList,
                                                                      maxDepth - 1);
                        else
                            siteTreeNode.Children = GetNodes<T>(siteTreeNode, pagedList);
                    }
                    else
                        siteTreeNode.Children = new List<SiteTreeNode<T>>();

                    siteTreeNode.Parent = parent;
                    list.Add(siteTreeNode);
                }
            });
            return list;
        }

        private bool ShowChildrenInAdminNav(Document document)
        {
            var documentTypeDefinition = DocumentMetadataHelper.GetMetadataByType(document.GetType());
            if (documentTypeDefinition != null)
                return documentTypeDefinition.ShowChildrenInAdminNav;
            return true;
        }

        private bool IsSortable(Document document)
        {
            var documentTypeDefinition = DocumentMetadataHelper.GetMetadataByType(document.GetType());
            if (documentTypeDefinition != null)
                return documentTypeDefinition.Sortable;
            return false;
        }

        private string GetNodeType<T>(T document) where T : Document
        {
            return document is Webpage
                       ? "Webpage"
                       : document is Layout
                             ? "Layout"
                             : document is MediaCategory
                                   ? "MediaCategory"
                                   : "";

        }
    }
}