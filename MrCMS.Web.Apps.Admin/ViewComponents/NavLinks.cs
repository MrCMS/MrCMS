using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Web.Apps.Admin.Services;

namespace MrCMS.Web.Apps.Admin.ViewComponents
{
    public class NavLinksViewComponent  :ViewComponent   
    {
        private readonly IAdminNavLinksService _service;

        public NavLinksViewComponent(IAdminNavLinksService service)
        {
            _service = service;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(_service.GetNavLinks());
        }
    }
}