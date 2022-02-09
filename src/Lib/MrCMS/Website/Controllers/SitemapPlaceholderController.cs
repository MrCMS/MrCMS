using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Website.Controllers
{
    public class SitemapPlaceholderController : MrCMSUIController
    {
        private readonly ISitemapPlaceholderUIService _sitemapPlaceholderUIService;
        private readonly IWebpageUIService _webpageUiService;

        public SitemapPlaceholderController(ISitemapPlaceholderUIService sitemapPlaceholderUIService, IWebpageUIService webpageUiService)
        {
            _sitemapPlaceholderUIService = sitemapPlaceholderUIService;
            _webpageUiService = webpageUiService;
        }

        public async Task<RedirectResult> Show(int id)
        {
            var page = await _webpageUiService.GetPage<SitemapPlaceholder>(id);
            return await _sitemapPlaceholderUIService.Redirect(page);
        }
    }
}