using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Admin.Infrastructure.Breadcrumbs;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.ViewComponents
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