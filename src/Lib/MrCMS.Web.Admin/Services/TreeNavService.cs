using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Admin.Services
{
    public class TreeNavService : ITreeNavService
    {
        private static readonly Dictionary<string, Type> TreeNavListings;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUrlHelper _urlHelper;
        private readonly ISession _session;

        static TreeNavService()
        {
            TreeNavListings = new Dictionary<string, Type>();
            var allTypes = TypeHelper.GetAllConcreteTypesAssignableFromGeneric(typeof(WebpageTreeNavListing<>));

            foreach (Type type in
                TypeHelper.GetAllConcreteMappedClassesAssignableFrom<Webpage>()
                    .Where(type => !type.ContainsGenericParameters))
            {
                var types = allTypes.FindAll(x =>
                    typeof(WebpageTreeNavListing<>).MakeGenericType(type).IsAssignableFrom(x));
                if (types.Any())
                {
                    TreeNavListings.Add(type.FullName, types.First());
                }
            }
        }

        public TreeNavService(ISession session, IServiceProvider serviceProvider, IUrlHelper urlHelper)
        {
            _session = session;
            _serviceProvider = serviceProvider;
            _urlHelper = urlHelper;
        }

        public async Task<AdminTree> GetWebpageNodes(int? id)
        {
            var webpageTreeNavListing = await GetListing(id);
            return await webpageTreeNavListing.GetTree(id);
        }

        public async Task<bool> WebpageHasChildren(int id)
        {
            var listing = await GetListing(id);
            return await listing.HasChildren(id);
        }

        public async Task<AdminTree> GetMediaCategoryNodes(int? id)
        {
            AdminTree adminTree = await GetSimpleAdminTree<MediaCategory>(id, "fa fa-picture");
            adminTree.RootContoller = "MediaCategory";
            return adminTree;
        }

        public async Task<AdminTree> GetLayoutNodes(int? id)
        {
            AdminTree adminTree = await GetSimpleAdminTree<Layout>(id, "fa fa-th-large");
            adminTree.RootContoller = "Layout";
            return adminTree;
        }

        private async Task<IWebpageTreeNavListing> GetListing(int? id)
        {
            var parent = id.HasValue ? await _session.GetAsync<Webpage>(id) : null;
            IWebpageTreeNavListing listing = null;
            if (parent != null && TreeNavListings.ContainsKey(parent.GetType().FullName))
            {
                listing =
                    _serviceProvider.GetRequiredService(TreeNavListings[parent.GetType().FullName]) as
                        IWebpageTreeNavListing;
            }

            return listing ?? _serviceProvider.GetRequiredService<DefaultWebpageTreeNavListing>();
        }

        private async Task<AdminTree> GetSimpleAdminTree<T>(int? id, string iconClass) where T : Document
        {
            var adminTree = new AdminTree();
            if (!id.HasValue)
            {
                adminTree.IsRootRequest = true;
            }

            IList<T> query =
                await _session.QueryOver<T>()
                    .Where(x => x.Parent.Id == id && (x.HideInAdminNav != true))
                    .OrderBy(x => x.Name)
                    .Asc.Cacheable()
                    .ListAsync();
            foreach (var doc in query)
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
                    HasChildren = await _session.QueryOver<T>().Where(arg => arg.Parent.Id == doc.Id).Cacheable()
                        .AnyAsync(),
                    CanAddChild = true,
                    IsPublished = true,
                    RevealInNavigation = true,
                    Url = _urlHelper.Action(typeof(T) == typeof(Layout) ? "Edit" : "Show", type.Name, new {id = doc.Id})
                };
                adminTree.Nodes.Add(node);
            }

            return adminTree;
        }
    }
}