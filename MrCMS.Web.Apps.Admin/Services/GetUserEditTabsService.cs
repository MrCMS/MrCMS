using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Apps.Admin.Models.UserEdit;

namespace MrCMS.Web.Apps.Admin.Services
{
    public class GetUserEditTabsService : IGetUserEditTabsService
    {
        private readonly IServiceProvider _kernel;

        public GetUserEditTabsService(IServiceProvider kernel)
        {
            _kernel = kernel;
        }

        public List<UserTabBase> GetEditTabs(User user)
        {
            var tabsToShow =
                TypeHelper.GetAllConcreteTypesAssignableFrom<UserTabBase>()
                    .Select(type => _kernel.GetService(type))
                    .OfType<UserTabBase>()
                    .Where(@base => @base.ShouldShow(user))
                    .ToList();

            var rootTabs = tabsToShow.Where(@base => @base.ParentType == null).OrderBy(@base => @base.Order).ToList();
            foreach (var tab in rootTabs)
            {
                AssignChildren(tab, tabsToShow);
            }

            return rootTabs;
        }

        private void AssignChildren(UserTabBase tab, List<UserTabBase> allTabs)
        {
            var tabGroup = tab as UserTabGroup;
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