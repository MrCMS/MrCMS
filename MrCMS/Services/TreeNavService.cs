using System;
using System.Linq;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Services
{
    public class TreeNavService : ITreeNavService
    {
        private readonly ISession _session;
        private readonly Site _site;

        public TreeNavService(ISession session, Site site)
        {
            _session = session;
            _site = site;
        }

        public AdminTree GetWebpageNodes(int? id)
        {
            var adminTree = new AdminTree {RootContoller = "Webpage"};
            var query = _session.QueryOver<Webpage>().Where(x => x.Parent.Id == id && x.Site.Id == _site.Id);
            int maxChildNodes = 1000;
            if (id.HasValue)
            {
                var parent = _session.Get<Webpage>(id);
                if (parent != null)
                {
                    var metaData = parent.GetMetadata();
                    maxChildNodes = metaData.MaxChildNodes;
                    query = ApplySort(metaData, query);
                }
            }
            else
            {
                adminTree.IsRootRequest = true;
                query = query.OrderBy(x => x.DisplayOrder).Asc;
            }

            var rowCount = query.Cacheable().RowCount();
            query.Take(maxChildNodes).Cacheable().List().ForEach(doc =>
                {
                    var documentMetadata = doc.GetMetadata();
                    var node = new AdminTreeNode
                        {
                            Id = doc.Id,
                            ParentId = doc.ParentId,
                            Name = doc.Name,
                            IconClass = documentMetadata.IconClass,
                            NodeType = "Webpage",
                            HasChildren = _session.QueryOver<Webpage>().Where(webpage => webpage.Parent.Id == doc.Id).Cacheable().Any(),
                            Sortable = documentMetadata.Sortable,
                            CanAddChild = doc.GetValidWebpageDocumentTypes().Any(),
                            IsPublished = doc.Published,
                            RevealInNavigation = doc.RevealInNavigation
                        };
                    adminTree.Nodes.Add(node);
                });
            if (rowCount > maxChildNodes)
            {
                adminTree.Nodes.Add(new AdminTreeNode
                    {
                        NumberMore = (rowCount - maxChildNodes),
                        IconClass = "icon-plus",
                        IsMoreLink = true,
                        ParentId = id
                    });
            }
            return adminTree;
        }

        private static IQueryOver<Webpage, Webpage> ApplySort(DocumentMetadata metaData, IQueryOver<Webpage, Webpage> query)
        {
            switch (metaData.SortBy)
            {
                case SortBy.DisplayOrder:
                    query = query.OrderBy(webpage => webpage.DisplayOrder).Asc;
                    break;
                case SortBy.DisplayOrderDesc:
                    query = query.OrderBy(webpage => webpage.DisplayOrder).Desc;
                    break;
                case SortBy.PublishedOn:
                    query =
                        query.OrderBy(Projections.Conditional(Restrictions.IsNull(Projections.Property<Webpage>(x => x.PublishOn)), Projections.Constant(1), Projections.Constant(0))).Desc.ThenBy(webpage => webpage.PublishOn)
                             .Asc;
                    break;
                case SortBy.PublishedOnDesc:
                    query =
                        query.OrderBy(Projections.Conditional(Restrictions.IsNull(Projections.Property<Webpage>(x => x.PublishOn)), Projections.Constant(1), Projections.Constant(0))).Desc.ThenBy(webpage => webpage.PublishOn)
                             .Desc;
                    break;
                case SortBy.CreatedOn:
                    query = query.OrderBy(webpage => webpage.CreatedOn).Asc;
                    break;
                case SortBy.CreatedOnDesc:
                    query = query.OrderBy(webpage => webpage.CreatedOn).Desc;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return query;
        }

        public AdminTree GetMediaCategoryNodes(int? id)
        {
            var adminTree = GetSimpleAdminTree<MediaCategory>(id, "icon-picture");
            adminTree.RootContoller = "MediaCategory";
            return adminTree;
        }

        public AdminTree GetLayoutNodes(int? id)
        {
            var adminTree = GetSimpleAdminTree<Layout>(id, "icon-th-large");
            adminTree.RootContoller = "Layout";
            return adminTree;
        }

        private AdminTree GetSimpleAdminTree<T>(int? id, string iconClass) where T : Document
        {
            var adminTree = new AdminTree();
            if (!id.HasValue)
            {
                adminTree.IsRootRequest = true;
            }
            var query =
                _session.QueryOver<T>()
                        .Where(x => x.Parent.Id == id && x.Site.Id == _site.Id && (x.HideInAdminNav == false || x.HideInAdminNav == null))
                        .OrderBy(x => x.Name)
                        .Asc.Cacheable()
                        .List();
            query.ForEach(doc =>
                {
                    var node = new AdminTreeNode
                        {
                            Id = doc.Id,
                            ParentId = doc.ParentId,
                            Name = doc.Name,
                            IconClass = iconClass,
                            NodeType = typeof(T).Name,
                            HasChildren = _session.QueryOver<T>().Where(arg => arg.Parent.Id == doc.Id).Cacheable().Any(),
                            CanAddChild = true,
                            IsPublished = true,
                            RevealInNavigation = true
                        };
                    adminTree.Nodes.Add(node);
                });
            return adminTree;
        }

    }
}