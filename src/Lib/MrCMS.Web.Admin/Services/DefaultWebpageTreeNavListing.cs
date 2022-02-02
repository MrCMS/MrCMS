using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using MrCMS.Web.Admin.Models;
using NHibernate;
using NHibernate.Criterion;

namespace MrCMS.Web.Admin.Services
{
    public class DefaultWebpageTreeNavListing : IWebpageTreeNavListing
    {
        private readonly ISession _session;
        private readonly ITreeNavService _treeNavService;
        private readonly IWebpageMetadataService _webpageMetadataService;
        private readonly IUrlHelper _urlHelper;
        private readonly IValidWebpageChildrenService _validWebpageChildrenService;

        public DefaultWebpageTreeNavListing(IValidWebpageChildrenService validWebpageChildrenService, ISession session,
            IUrlHelper urlHelper, ITreeNavService treeNavService, IWebpageMetadataService webpageMetadataService)
        {
            _validWebpageChildrenService = validWebpageChildrenService;
            _session = session;
            _urlHelper = urlHelper;
            _treeNavService = treeNavService;
            _webpageMetadataService = webpageMetadataService;
        }

        public async Task<AdminTree> GetTree(int? id)
        {
            Webpage parent = id.HasValue ? await _session.GetAsync<Webpage>(id) : null;
            var adminTree = new AdminTree
            {
                RootContoller = "Webpage",
                IsRootRequest = parent == null
            };
            int maxChildNodes = parent == null ? 1000 : _webpageMetadataService.GetMetadata(parent).MaxChildNodes;
            IQueryOver<Webpage, Webpage> query = GetQuery(parent);

            int rowCount = await GetRowCount(query);
            var webpages = await query.Take(maxChildNodes).Cacheable().ListAsync();
            foreach (var doc in webpages)
            {
                WebpageMetadata webpageMetadata = _webpageMetadataService.GetMetadata(doc);
                var node = new AdminTreeNode
                {
                    Id = doc.Id,
                    ParentId = doc.Parent?.Id,
                    Name = doc.Name,
                    IconClass = webpageMetadata.IconClass,
                    NodeType = "Webpage",
                    Type = webpageMetadata.Type.FullName,
                    HasChildren = await _treeNavService.WebpageHasChildren(doc.Id),
                    Sortable = webpageMetadata.Sortable,
                    CanAddChild = await _validWebpageChildrenService.AnyValidWebpageTypes(doc),
                    IsPublished = doc.Published,
                    RevealInNavigation = doc.RevealInNavigation,
                    Url = _urlHelper.Action("Edit", "Webpage", new {id = doc.Id})
                };
                adminTree.Nodes.Add(node);
            }

            ;
            if (rowCount > maxChildNodes)
            {
                adminTree.Nodes.Add(new AdminTreeNode
                {
                    IconClass = "fa fa-plus",
                    IsMoreLink = true,
                    ParentId = id,
                    Name = (rowCount - maxChildNodes) + " More",
                    Url = _urlHelper.Action("Search", "WebpageSearch", new {parentId = id}),
                });
            }

            return adminTree;
        }

        public async Task<bool> HasChildren(int id)
        {
            var parent = await _session.GetAsync<Webpage>(id);
            IQueryOver<Webpage, Webpage> query = GetQuery(parent);
            return await GetRowCount(query) > 0;
        }

        private static async Task<int> GetRowCount(IQueryOver<Webpage, Webpage> query)
        {
            return await query.Cacheable().RowCountAsync();
        }

        private IQueryOver<Webpage, Webpage> GetQuery(Webpage parent)
        {
            IQueryOver<Webpage, Webpage> query = _session.QueryOver<Webpage>();
            if (parent != null)
            {
                query = query.Where(x => x.Parent.Id == parent.Id);
                WebpageMetadata metaData = _webpageMetadataService.GetMetadata(parent);
                query = ApplySort(metaData, query);
            }
            else
            {
                query = query.Where(x => x.Parent == null);
                query = query.OrderBy(x => x.DisplayOrder).Asc;
            }

            return query;
        }

        private static IQueryOver<Webpage, Webpage> ApplySort(WebpageMetadata metaData,
            IQueryOver<Webpage, Webpage> query)
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
                        query.OrderBy(
                                Projections.Conditional(
                                    Restrictions.IsNull(Projections.Property<Webpage>(x => x.PublishOn)),
                                    Projections.Constant(1), Projections.Constant(0)))
                            .Desc.ThenBy(webpage => webpage.PublishOn)
                            .Asc;
                    break;
                case SortBy.PublishedOnDesc:
                    query =
                        query.OrderBy(
                                Projections.Conditional(
                                    Restrictions.IsNull(Projections.Property<Webpage>(x => x.PublishOn)),
                                    Projections.Constant(1), Projections.Constant(0)))
                            .Desc.ThenBy(webpage => webpage.PublishOn)
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