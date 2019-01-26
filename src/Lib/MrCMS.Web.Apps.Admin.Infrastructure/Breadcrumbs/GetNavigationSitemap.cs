using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public class GetNavigationSitemap : IGetNavigationSitemap
    {
        private readonly IBreadcrumbAccessChecker _breadcrumbAccessChecker;
        private readonly IGetNavigationTypes _getNavigationTypes;
        private readonly IServiceProvider _serviceProvider;

        public GetNavigationSitemap(IGetNavigationTypes getNavigationTypes, IServiceProvider serviceProvider,
            IBreadcrumbAccessChecker breadcrumbAccessChecker)
        {
            _getNavigationTypes = getNavigationTypes;
            _serviceProvider = serviceProvider;
            _breadcrumbAccessChecker = breadcrumbAccessChecker;
        }

        public Sitemap GetNavigation()
        {
            return new Sitemap {Nodes = GetNodes()};
        }

        private List<SitemapNode> GetNodes()
        {
            var rootNavTypes = _getNavigationTypes.GetRootNavTypes();

            return FilterAndSort(rootNavTypes);
        }

        private List<SitemapNode> FilterAndSort(IEnumerable<Type> types)
        {
            List<SitemapNode> sitemapNodes = new List<SitemapNode>();
            foreach (var type in types)
            {
                sitemapNodes.Add(GetNodeForType(type));
            }

            return sitemapNodes.Where(x => x != null).OrderBy(x => x.Order).ToList();
        }

        private SitemapNode GetNodeForType(Type type)
        {
            var breadcrumb = (Breadcrumb) _serviceProvider.GetService(type);
            if (breadcrumb?.IsNav != true)
                return null;
            if (!breadcrumb.IsPlaceHolder && !_breadcrumbAccessChecker.CanAccess(breadcrumb))
                return null;

            var node = new SitemapNode(breadcrumb);

            var types = _getNavigationTypes.GetChildren(type);
            node.AddChildren(FilterAndSort(types));

            return node;
        }
    }
}