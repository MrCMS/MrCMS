using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public class RedirectController : MrCMSUIController
    {
        private readonly IWebpageUIService _webpageUiService;

        public RedirectController(IWebpageUIService webpageUiService)
        {
            _webpageUiService = webpageUiService;
        }

        public async Task<IActionResult> Show(int id)
        {
            var page = await _webpageUiService.GetPage<Redirect>(id);
            if (page == null)
                return StatusCode(404);

            if (page.Permanent)
                return RedirectPermanent(page.RedirectUrl);

            return Redirect(page.RedirectUrl);
        }

        public RedirectResult HomePage()
        {
            return RedirectPermanent("/");
        }
    }
}