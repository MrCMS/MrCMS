using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Attributes;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.ModelBinders;
using MrCMS.Web.Apps.Core.Models.Search;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services.Search;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class SearchPageController : MrCMSAppUIController<MrCMSCoreApp>
    {
        private readonly IWebpageSearchService _webpageSearchService;
        private readonly IWebpageUIService _webpageUiService;

        public SearchPageController(IWebpageSearchService webpageSearchService, IWebpageUIService webpageUiService)
        {
            _webpageSearchService = webpageSearchService;
            _webpageUiService = webpageUiService;
        }
        [CanonicalLinks]
        public async Task<ActionResult> Show(int id, [ModelBinder(typeof(WebpageSearchQueryModelBinder))]WebpageSearchQuery model)
        {
            var page = await _webpageUiService.GetPage<SearchPage>(id);
            ViewData["webpageSearchQueryModel"] = model;

            ViewData["searchResults"] = await _webpageSearchService.Search(model);
            return View(page);
        }
    }
}
