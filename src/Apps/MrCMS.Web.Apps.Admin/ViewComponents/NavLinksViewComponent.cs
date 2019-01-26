using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Infrastructure.Breadcrumbs;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.ViewComponents
{
    public class NavLinksViewComponent : ViewComponent
    {
        private readonly IGetNavigationSitemap _getNavigation;

        public NavLinksViewComponent( IGetNavigationSitemap getNavigation)
        {
            _getNavigation = getNavigation;
        }
        public IViewComponentResult Invoke()
        {
            return View(_getNavigation.GetNavigation());
        }
    }
}