using System;
using System.Collections.Generic;
using System.Linq;

namespace MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs
{
    public class GetBreadcrumbs : IGetBreadcrumbs
    {
        private readonly IGetBreadcrumbTypes _getBreadcrumbTypes;
        private readonly IServiceProvider _serviceProvider;
        private readonly IPageHeaderBreadcrumbBuilder _pageHeaderBreadcrumbBuilder;

        public GetBreadcrumbs(IGetBreadcrumbTypes getBreadcrumbTypes, IServiceProvider serviceProvider,
            IPageHeaderBreadcrumbBuilder pageHeaderBreadcrumbBuilder)
        {
            _pageHeaderBreadcrumbBuilder = pageHeaderBreadcrumbBuilder;
            _getBreadcrumbTypes = getBreadcrumbTypes;
            _serviceProvider = serviceProvider;
        }

        public List<PageHeaderBreadcrumb> Get(Type type, int? id)
        {
            var siteMapNodes = new List<PageHeaderBreadcrumb>();
            if (type == null)
            {
                return siteMapNodes;
            }

            var types = _getBreadcrumbTypes.GetHierarchy(type);

            var currentId = id;

            for (var index = 0; index < types.Count;)
            {
                var nodeType = types[index];
                var breadcrumb = (Breadcrumb)_serviceProvider.GetService(nodeType);
                if (currentId.HasValue)
                {
                    breadcrumb.Id = currentId;
                }

                breadcrumb.Populate();
                if (!breadcrumb.ShouldSkip)
                {
                    siteMapNodes.Insert(0, _pageHeaderBreadcrumbBuilder.Build(breadcrumb));
                }

                // if it's hierarchical and currently has an id, we'll not move to the parent yet
                if (breadcrumb.Id.HasValue && breadcrumb.Hierarchical)
                {
                    index--;
                }

                currentId = breadcrumb.ParentId;
                index++;
            }

            return siteMapNodes.ToList();
        }

        public List<PageHeaderBreadcrumb> Get(string controllerName, string actionName, int? id)
        {
            var type = _getBreadcrumbTypes.FindBreadcrumbType(controllerName, actionName);

            return Get(type, id);
        }
    }
}