using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<List<PageHeaderBreadcrumb>> Get(Type type, IDictionary<string, object> actionArguments)
        {
            var siteMapNodes = new List<PageHeaderBreadcrumb>();
            if (type == null)
            {
                return siteMapNodes;
            }

            var types = _getBreadcrumbTypes.GetHierarchy(type);

            var currentArguments = actionArguments;

            for (var index = 0; index < types.Count;)
            {
                var nodeType = types[index];
                var breadcrumb = (Breadcrumb)_serviceProvider.GetService(nodeType);
                if (currentArguments != null)
                {
                    breadcrumb.ActionArguments = currentArguments;
                }


                await breadcrumb.Populate();
                if (!breadcrumb.ShouldSkip)
                {
                    siteMapNodes.Insert(0, _pageHeaderBreadcrumbBuilder.Build(breadcrumb));
                }

                // if it's hierarchical and currently has an id, we'll not move to the parent yet
                if (breadcrumb.Id.HasValue && breadcrumb.Hierarchical)
                {
                    index--;
                }

                currentArguments = breadcrumb.ParentActionArguments;
                index++;
            }

            return siteMapNodes.ToList();
        }

        public async Task<List<PageHeaderBreadcrumb>> Get(string controllerName, string actionName,
            IDictionary<string, object> actionArguments)
        {
            var type = _getBreadcrumbTypes.FindBreadcrumbType(controllerName, actionName);

            return await Get(type, actionArguments);
        }
    }
}