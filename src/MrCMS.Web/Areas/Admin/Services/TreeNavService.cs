using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class TreeNavService : ITreeNavService
    {
        private static readonly Dictionary<string, Type> TreeNavListings;
        private readonly IRepository<Document> _repository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUrlHelper _urlHelper;

        static TreeNavService()
        {
            TreeNavListings = new Dictionary<string, Type>();

            foreach (
                Type type in
                    TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                        .Where(type => !type.ContainsGenericParameters))
            {
                var types =
                    TypeHelper.GetAllConcreteTypesAssignableFrom(typeof(WebpageTreeNavListing<>).MakeGenericType(type));
                if (types.Any())
                {
                    TreeNavListings.Add(type.FullName, types.First());
                }
            }
        }

        public TreeNavService(IRepository<Document> repository, IServiceProvider serviceProvider, IUrlHelper urlHelper)
        {
            _repository = repository;
            _serviceProvider = serviceProvider;
            _urlHelper = urlHelper;
        }

        public AdminTree GetWebpageNodes(int? id)
        {
            var webpageTreeNavListing = GetListing(id);
            //using (MiniProfiler.Current.Step(string.Format("Get Tree - {0} ({1})", webpageTreeNavListing.GetType().Name, id)))
            return webpageTreeNavListing.GetTree(id);
        }

        public bool WebpageHasChildren(int id)
        {
            return GetListing(id).HasChildren(id);
        }

        public AdminTree GetMediaCategoryNodes(int? id)
        {
            AdminTree adminTree = GetSimpleAdminTree<MediaCategory>(id, "fa fa-picture");
            adminTree.RootContoller = "MediaCategory";
            return adminTree;
        }

        public AdminTree GetLayoutNodes(int? id)
        {
            AdminTree adminTree = GetSimpleAdminTree<Layout>(id, "fa fa-th-large");
            adminTree.RootContoller = "Layout";
            return adminTree;
        }

        private IWebpageTreeNavListing GetListing(int? id)
        {
            //using (MiniProfiler.Current.Step("Get Listing"))
            {
                var parent = id.HasValue ? _repository.GetDataSync<Document, Webpage>(id.Value) : null;
                IWebpageTreeNavListing listing = null;
                if (parent != null && TreeNavListings.ContainsKey(parent.GetType().FullName))
                {
                    listing = _serviceProvider.GetRequiredService(TreeNavListings[parent.GetType().FullName]) as IWebpageTreeNavListing;
                }
                return listing ?? _serviceProvider.GetRequiredService<DefaultWebpageTreeNavListing>();
            }
        }

        private AdminTree GetSimpleAdminTree<T>(int? id, string iconClass) where T : Document
        {
            var adminTree = new AdminTree();
            if (!id.HasValue)
            {
                adminTree.IsRootRequest = true;
            }
            IList<T> query =
                _repository.Readonly<T>()
                    .Where(x => x.Parent.Id == id && (x.HideInAdminNav != true))
                    .OrderBy(x => x.Name)
                    .ToList();
            query.ForEach(doc =>
            {
                var type = typeof(T);
                var node = new AdminTreeNode
                {
                    Id = doc.Id,
                    ParentId = doc.ParentId,
                    Name = doc.Name,
                    IconClass = iconClass,
                    NodeType = type.Name,
                    Type = type.FullName,
                    HasChildren = _repository.Readonly().Any(arg => arg.ParentId == doc.Id),
                    CanAddChild = true,
                    IsPublished = true,
                    RevealInNavigation = true,
                    Url = _urlHelper.Action(typeof(T) == typeof(Layout) ? "Edit" : "Show", type.Name, new { id = doc.Id })
                };
                adminTree.Nodes.Add(node);
            });
            return adminTree;
        }
    }
}