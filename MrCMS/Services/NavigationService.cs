using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Indexes;
using MrCMS.Entities.Multisite;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Indexing.Querying;
using MrCMS.Models;
using MrCMS.Website;
using Document = MrCMS.Entities.Documents.Document;

namespace MrCMS.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IDocumentService _documentService;
        private readonly ISearcher<Document, DocumentIndexDefinition> _documentSearcher;

        public NavigationService(IDocumentService documentService, ISearcher<Document, DocumentIndexDefinition> documentSearcher)
        {
            _documentService = documentService;
            _documentSearcher = documentSearcher;
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
            tree.Children = GetNodes<Webpage>(tree, _documentSearcher.IndexSearcher.Search(GetRootEntities<Webpage>(), int.MaxValue).ScoreDocs, null, maxDepth: depth);

            return tree;
        }

        private Query GetRootEntities<T>()
        {
            var rootEntities = new BooleanQuery
                {
                    {new TermQuery(new Term(DocumentIndexDefinition.Type.FieldName, typeof (T).FullName)), Occur.MUST},
                    {new TermQuery(new Term(DocumentIndexDefinition.IsRootEntity.FieldName, true.ToString())), Occur.MUST}
                };

            return rootEntities;
        }

        private Query GetEntities<T>(int parentId)
        {
            var rootEntities = new BooleanQuery
                {
                    {new TermQuery(new Term(DocumentIndexDefinition.Type.FieldName, typeof (T).FullName)), Occur.MUST},
                    {new TermQuery(new Term(DocumentIndexDefinition.ParentId.FieldName, parentId.ToString())), Occur.MUST}
                };

            return rootEntities;
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
            tree.Children = GetNodes<MediaCategory>(tree, _documentSearcher.IndexSearcher.Search(GetRootEntities<MediaCategory>(), int.MaxValue).ScoreDocs, null);

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

            tree.Children = GetNodes<Layout>(tree, _documentSearcher.IndexSearcher.Search(GetRootEntities<Layout>(), int.MaxValue).ScoreDocs, null);
            return tree;
        }

        public IEnumerable<SelectListItem> GetParentsList()
        {
            var selectListItems = GetPageListItems(GetWebsiteTree().Children, 2).ToList();
            selectListItems.Insert(0, new SelectListItem { Selected = false, Text = "Root", Value = "" });
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


        private List<SiteTreeNode<T>> GetNodes<T>(SiteTreeNode parent, IEnumerable<ScoreDoc> docs, int? parentId, int? maxChildren = null, int? maxDepth = null) where T : Document
        {
            var list = new List<SiteTreeNode<T>>();

            if (maxChildren.HasValue)
            {
                docs = docs.Take(maxChildren.Value);
            }
            docs.ForEach(doc =>
                              {
                                  Lucene.Net.Documents.Document document = _documentSearcher.IndexSearcher.Doc(doc.Doc);
                                  if (document.GetValues(DocumentIndexDefinition.ShowInNav.FieldName).Contains(true.ToString()))
                                  {
                                      int id = Convert.ToInt32(document.GetValues(DocumentIndexDefinition.Id.FieldName).First());
                                      TopDocs topDocs = _documentSearcher.IndexSearcher.Search(GetEntities<T>(id), int.MaxValue);
                                      var count = topDocs.ScoreDocs.Count();
                                      string publishOn = document.Get(DocumentIndexDefinition.PublishOn.FieldName);
                                      var siteTreeNode = new SiteTreeNode<T>
                                                             {
                                                                 Total =
                                                                     count,
                                                                 IconClass = document.Get(DocumentIndexDefinition.IconClass.FieldName),
                                                                 Id = id,
                                                                 ParentId = parentId,
                                                                 Name = document.Get(DocumentIndexDefinition.Name.FieldName),
                                                                 NodeType = GetNodeType(document),
                                                                 Sortable = document.Get(DocumentIndexDefinition.IsSortable.FieldName) == true.ToString(),
                                                                 CanAddChild = document.Get(DocumentIndexDefinition.CanAddChild.FieldName) == true.ToString(),
                                                                 IsPublished = GetNodeType(document) != "Webpage" || !string.IsNullOrWhiteSpace(publishOn) && DateTools.StringToDate(publishOn) <= CurrentRequestData.Now,
                                                                 RevealInNavigation = document.Get(DocumentIndexDefinition.RevealInNavigation.FieldName) == true.ToString()
                                                             };
                                      if (document.GetValues(DocumentIndexDefinition.ShowChildrenInNav.FieldName).Contains(true.ToString()))
                                      {
                                          if (maxDepth.HasValue)
                                          {
                                              if (maxDepth.Value == 0)
                                              {
                                                  siteTreeNode.Children = new List<SiteTreeNode<T>>();
                                              }
                                              else
                                              {
                                                  siteTreeNode.Children = GetNodes<T>(siteTreeNode,
                                                      topDocs.ScoreDocs
                                                                                   , id,
                                                                                   Convert.ToInt32(document.Get(DocumentIndexDefinition.MaxChildNodes.FieldName)),
                                                                                   maxDepth - 1);
                                              }
                                          }
                                          else
                                          {
                                              siteTreeNode.Children = GetNodes<T>(siteTreeNode,
                                                                                  topDocs
                                                                                                   .ScoreDocs, id,
                                                                                  Convert.ToInt32(document.Get(DocumentIndexDefinition.MaxChildNodes.FieldName)));
                                          }
                                      }
                                      else
                                      {
                                          siteTreeNode.Children = new List<SiteTreeNode<T>>();
                                      }
                                      siteTreeNode.Parent = parent;
                                      list.Add(siteTreeNode);
                                  }
                              });
            return list;
        }

        private bool ShowInNav(Document document)
        {
            return !(document is MediaCategory) || !(document as MediaCategory).HideInAdminNav;
        }

        private bool ShowChildrenInAdminNav(Document document)
        {
            var documentTypeDefinition = DocumentMetadataHelper.GetMetadataByType(document.GetType());
            if (documentTypeDefinition != null)
                return documentTypeDefinition.ShowChildrenInAdminNav;
            return true;
        }

        private bool IsSortable(int? documentId)
        {
            if (documentId.HasValue)
            {
                var document = _documentService.GetDocument<Document>(documentId.Value);
                var documentTypeDefinition = DocumentMetadataHelper.GetMetadataByType(document.GetType());
                if (documentTypeDefinition != null)
                    return documentTypeDefinition.Sortable;
            }
            return false;
        }

        private string GetNodeType(Lucene.Net.Documents.Document document)
        {
            string[] values = document.GetValues(DocumentIndexDefinition.Type.FieldName);
            return values.Contains(typeof(Webpage).FullName)
                       ? "Webpage"
                       : values.Contains(typeof(Layout).FullName)
                             ? "Layout"
                             : values.Contains(typeof(MediaCategory).FullName)
                                   ? "MediaCategory"
                                   : "";

        }
    }
}