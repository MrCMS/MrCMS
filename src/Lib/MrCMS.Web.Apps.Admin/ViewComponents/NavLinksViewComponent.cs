using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.ViewComponents
{
    public class NavLinksViewComponent : ViewComponent
    {
        private readonly IAdminNavLinksService _service;

        public NavLinksViewComponent(IAdminNavLinksService service)
        {
            _service = service;
        }
        public IViewComponentResult Invoke()
        {
            return View(_service.GetNavLinks());
        }
    }
}