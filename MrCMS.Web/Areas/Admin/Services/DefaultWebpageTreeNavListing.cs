using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class DefaultWebpageTreeNavListing : IWebpageTreeNavListing
    {
        private readonly IValidWebpageChildrenService _validWebpageChildrenService;
        private readonly ISession _session;
        private readonly UrlHelper _urlHelper;
        private readonly ITreeNavService _treeNavService;

        public DefaultWebpageTreeNavListing(IValidWebpageChildrenService validWebpageChildrenService, ISession session, UrlHelper urlHelper,ITreeNavService treeNavService)
        {
            _validWebpageChildrenService = validWebpageChildrenService;
            _session = session;
            _urlHelper = urlHelper;
            _treeNavService = treeNavService;
        }

        public AdminTree GetTree(int? id)
        {
            var adminTree = new AdminTree { RootContoller = "Webpage" };
            var query = _session.QueryOver<Webpage>().Where(x => x.Parent.Id == id);
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
                    Type = documentMetadata.Type.FullName,
                    HasChildren = _treeNavService.GetWebpageNodes(doc.Id).Nodes.Any(),
                    Sortable = documentMetadata.Sortable,
                    CanAddChild = _validWebpageChildrenService.AnyValidWebpageDocumentTypes(doc),
                    IsPublished = doc.Published,
                    RevealInNavigation = doc.RevealInNavigation,
                    Url = _urlHelper.Action("Edit", "Webpage", new { id = doc.Id })
                };
                adminTree.Nodes.Add(node);
            });
            if (rowCount > maxChildNodes)
            {
                adminTree.Nodes.Add(new AdminTreeNode
                {
                    IconClass = "icon-plus",
                    IsMoreLink = true,
                    ParentId = id,
                    Name = (rowCount - maxChildNodes) + " More",
                    Url = _urlHelper.Action("Index", "Search", new RouteValueDictionary { { "Parent.Id", id } }),
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
    }
}