using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models.UserEdit;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class GetUserEditTabsService : IGetUserEditTabsService
    {
        private readonly IKernel _kernel;

        public GetUserEditTabsService(IKernel kernel)
        {
            _kernel = kernel;
        }

        public List<UserTabBase> GetEditTabs(User user)
        {
            var tabsToShow =
                TypeHelper.GetAllConcreteTypesAssignableFrom<UserTabBase>()
                    .Select(type => _kernel.Get(type))
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