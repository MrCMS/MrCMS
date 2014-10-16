using System.Collections.Generic;
using System.Linq;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Web.Areas.Admin.Models.WebpageEdit;
using Ninject;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class GetWebpageEditTabsService : IGetWebpageEditTabsService
    {
        private readonly IKernel _kernel;

        public GetWebpageEditTabsService(IKernel kernel)
        {
            _kernel = kernel;
        }

        public List<WebpageTabBase> GetEditTabs(Webpage page)
        {
            var tabsToShow =
                TypeHelper.GetAllConcreteTypesAssignableFrom<WebpageTabBase>()
                    .Select(type => _kernel.Get(type))
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