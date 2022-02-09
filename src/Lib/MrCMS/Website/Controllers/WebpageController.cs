using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public class WebpageController : MrCMSUIController
    {
        private readonly IWebpageUIService _webpageUiService;

        public WebpageController(IWebpageUIService webpageUiService)
        {
            _webpageUiService = webpageUiService;
        }
        [CanonicalLinks]
        public async Task<ViewResult> Show(int id)
        {
            var page = await _webpageUiService.GetPage<Webpage>(id);
            return View(page);
        }
    }
}