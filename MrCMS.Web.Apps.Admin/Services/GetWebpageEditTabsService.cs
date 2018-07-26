using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models.WebpageEdit;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class GetWebpageEditTabsService : IGetWebpageEditTabsService
    {
        private readonly IServiceProvider _serviceProvider;

        public GetWebpageEditTabsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<WebpageTabBase> GetEditTabs(Webpage page)
        {
            var tabsToShow =
                TypeHelper.GetAllConcreteTypesAssignableFrom<WebpageTabBase>()
                    .Select(type => _serviceProvider.GetService(type))
                    .OfType<WebpageTabBase>()
                    .Where(@base => @base.ShouldShow(page))
                    .ToList();

            var rootTabs = tabsToShow.Where(@base => @base.ParentType == null).OrderBy(@base => @base.Order).ToList();
            foreach (var tab in rootTabs)
            {
                AssignChildren(tab, tabsToShow);
            }

            return rootTabs;
        }

        private void AssignChildren(WebpageTabBase tab, List<WebpageTabBase> allTabs)
        {
            var tabGroup = tab as WebpageTabGroup;
            if (tabGroup == null)
            {
                return;
            }
            var children =
                allTabs.Where(x => x.ParentType == tabGroup.GetType()).OrderBy(@base => @base.Order).ToList();
            tabGroup.SetChildren(children);
            foreach (var tabBase in children)
            {
                AssignChildren(tabBase, allTabs);
            }
        }
    }
}