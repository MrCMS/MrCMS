using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Models;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class DefaultWebpageTreeNavListing : IWebpageTreeNavListing
    {
        private readonly ITreeNavService _treeNavService;
        private readonly IUrlHelper _urlHelper;
        private readonly IValidWebpageChildrenService _validWebpageChildrenService;
        private readonly IRepository<Webpage> _repository;

        public DefaultWebpageTreeNavListing(IValidWebpageChildrenService validWebpageChildrenService, IRepository<Webpage> repository,
            IUrlHelper urlHelper, ITreeNavService treeNavService)
        {
            _validWebpageChildrenService = validWebpageChildrenService;
            _repository = repository;
            _urlHelper = urlHelper;
            _treeNavService = treeNavService;
        }

        public AdminTree GetTree(int? id)
        {
            Webpage parent = id.HasValue ? _repository.LoadSync(id.Value) : null;
            var adminTree = new AdminTree
            {
                RootContoller = "Webpage",
                IsRootRequest = parent == null
            };
            int maxChildNodes = parent == null ? 1000 : parent.GetMetadata().MaxChildNodes;
            var query = GetQuery(parent);

            int rowCount = GetRowCount(query);
            query.Take(maxChildNodes).ToList().ForEach(doc =>
            {
                DocumentMetadata documentMetadata = doc.GetMetadata();
                var node = new AdminTreeNode
                {
                    Id = doc.Id,
                    ParentId = doc.ParentId,
                    Name = doc.Name,
                    IconClass = documentMetadata.IconClass,
                    NodeType = "Webpage",
                    Type = documentMetadata.Type.FullName,
                    HasChildren = _treeNavService.WebpageHasChildren(doc.Id),
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
                    IconClass = "fa fa-plus",
                    IsMoreLink = true,
                    ParentId = id,
                    Name = (rowCount - maxChildNodes) + " More",
                    Url = _urlHelper.Action("Search", "WebpageSearch", new { parentId = id }),
                });
            }
            return adminTree;
        }

        public bool HasChildren(int id)
        {
            var parent = _repository.LoadSync(id);
            var query = GetQuery(parent);
            return GetRowCount(query) > 0;
        }

        private static int GetRowCount(IQueryable< Webpage> query)
        {
            return query.Count();
        }

        private IQueryable<Webpage> GetQuery(Webpage parent)
        {
            var query = _repository.Query();
            if (parent != null)
            {
                query = query.Where(x => x.ParentId == parent.Id);
                DocumentMetadata metaData = parent.GetMetadata();
                query = ApplySort(metaData, query);
            }
            else
            {
                query = query.Where(x => x.ParentId == null);
                query = query.OrderBy(x => x.DisplayOrder);
            }
            return query;
        }

        private static IQueryable<Webpage> ApplySort(DocumentMetadata metaData,
            IQueryable<Webpage> query)
        {
            switch (metaData.SortBy)
            {
                case SortBy.DisplayOrder:
                    query = query.OrderBy(webpage => webpage.DisplayOrder);
                    break;
                case SortBy.DisplayOrderDesc:
                    query = query.OrderByDescending(webpage => webpage.DisplayOrder);
                    break;
                case SortBy.PublishedOn:
                    query =
                        query.OrderByDescending(x => x.Published).ThenBy(webpage => webpage.PublishOn);
                    break;
                case SortBy.PublishedOnDesc:
                    query =
                        query.OrderByDescending(x => x.Published).ThenByDescending(webpage => webpage.PublishOn);
                    break;
                case SortBy.CreatedOn:
                    query = query.OrderBy(webpage => webpage.CreatedOn);
                    break;
                case SortBy.CreatedOnDesc:
                    query = query.OrderByDescending(webpage => webpage.CreatedOn);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return query;
        }
    }
}