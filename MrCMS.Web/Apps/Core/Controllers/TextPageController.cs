using System.Web.Mvc;
using MrCMS.Attributes;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.ModelBinders;
using MrCMS.Web.Apps.Core.Models.Search;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Web.Apps.Core.Services.Search;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class TextPageController : MrCMSAppUIController<CoreApp>
    {
        private readonly IWebpageSearchService _webpageSearchService;
        private readonly IWebpageUIService _uiService;

        public TextPageController(IWebpageSearchService webpageSearchService, IWebpageUIService uiService)
        {
            _webpageSearchService = webpageSearchService;
            _uiService = uiService;
        }

        public ActionResult Show(TextPage page)
        {
            return _uiService.GetContent(this, page, helper => helper.Action("Internal", "Textpage", new { page }));
        }

        [CanonicalLinks]
        public ActionResult Internal(TextPage page)
        {
            return View(page);
        }
    }
}