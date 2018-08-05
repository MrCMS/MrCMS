using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models.Tabs;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class GetWebpageEditTabsService : IGetWebpageEditTabsService
    {
        private readonly IServiceProvider _serviceProvider;

        public GetWebpageEditTabsService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<AdminTabBase<Webpage>> GetEditTabs(IHtmlHelper html, Webpage page)
        {
            var tabsToShow =
                TypeHelper.GetAllConcreteTypesAssignableFrom<AdminTabBase<Webpage>>()
                    .Select(type => _serviceProvider.GetService(type))
                    .OfType<AdminTabBase<Webpage>>()
                    .Where(@base => @base.ShouldShow(html, page))
                    .ToList();

            var rootTabs = tabsToShow.Where(@base => @base.ParentType == null).OrderBy(@base => @base.Order).ToList();
            foreach (var tab in rootTabs)
            {
                AssignChildren(tab, tabsToShow);
            }

            return rootTabs;
        }

        private void AssignChildren(AdminTabBase<Webpage> tab, List<AdminTabBase<Webpage>> allTabs)
        {
            var tabGroup = tab as AdminTabGroup<Webpage>;
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