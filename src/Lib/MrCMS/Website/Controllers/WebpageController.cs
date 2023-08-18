using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;
using StackExchange.Profiling;

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
            using (MiniProfiler.Current.Step("WebpageController.Show"))
            {
                var page = await _webpageUiService.GetPage<Webpage>(id);
                return View(page);
            }
        }
    }
}