using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Admin.Models;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            AdminTree adminTree = await GetMediaCategoryAdminTree(id);
            adminTree.RootContoller = "MediaCategory";
            return adminTree;
        }

        public async Task<AdminTree> GetLayoutNodes(int? id)
        {
            AdminTree adminTree = await GetLayoutAdminTree(id);
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

        private async Task<AdminTree> GetMediaCategoryAdminTree(int? id)
        {
            var adminTree = new AdminTree();
            if (!id.HasValue)
            {
                adminTree.IsRootRequest = true;
            }

            IList<MediaCategory> query =
                await _session.QueryOver<MediaCategory>()
                    .Where(x => x.Parent.Id == id && (x.HideInAdminNav != true))
                    .OrderBy(x => x.Name)
                    .Asc.Cacheable()
                    .ListAsync();
            foreach (var doc in query)
            {
                var type = typeof(MediaCategory);
                var node = new AdminTreeNode
                {
                    Id = doc.Id,
                    ParentId = doc.Parent?.Id,
                    Name = doc.Name,
                    IconClass = "fa fa-picture",
                    NodeType = type.Name,
                    Type = type.FullName,
                    HasChildren = await _session.QueryOver<MediaCategory>().Where(arg => arg.Parent.Id == doc.Id).Cacheable()
                        .AnyAsync(),
                    CanAddChild = true,
                    IsPublished = true,
                    RevealInNavigation = true,
                    Url = _urlHelper.Action("Show", type.Name, new { id = doc.Id })
                };
                adminTree.Nodes.Add(node);
            }

            return adminTree;
        }

        private async Task<AdminTree> GetLayoutAdminTree(int? id)
        {
            var adminTree = new AdminTree();
            if (!id.HasValue)
            {
                adminTree.IsRootRequest = true;
            }

            IList<Layout> query =
                await _session.QueryOver<Layout>()
                    .Where(x => x.Parent.Id == id && (x.HideInAdminNav != true))
                    .OrderBy(x => x.Name)
                    .Asc.Cacheable()
                    .ListAsync();
            foreach (var doc in query)
            {
                var type = typeof(Layout);
                var node = new AdminTreeNode
                {
                    Id = doc.Id,
                    ParentId = doc.Parent?.Id,
                    Name = doc.Name,
                    IconClass = "fa fa-th-large",
                    NodeType = type.Name,
                    Type = type.FullName,
                    HasChildren = await _session.QueryOver<Layout>().Where(arg => arg.Parent.Id == doc.Id).Cacheable()
                        .AnyAsync(),
                    CanAddChild = true,
                    IsPublished = true,
                    RevealInNavigation = true,
                    Url = _urlHelper.Action("Edit", type.Name, new { id = doc.Id })
                };
                adminTree.Nodes.Add(node);
            }

            return adminTree;
        }
    }
}