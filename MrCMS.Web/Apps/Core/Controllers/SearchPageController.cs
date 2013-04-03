using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Services;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class SearchPageController : MrCMSAppUIController<CoreApp>
    {
        private readonly ISearchService _documentService;

        public SearchPageController(ISearchService documentService)
        {
            _documentService = documentService;
        }

        public ActionResult Show(SearchPage page)
        {
            int pageVal;
            ViewData["term"] = Request["q"];
            int pageNum = string.IsNullOrWhiteSpace(Request["p"])
                               ? 1
                               : int.TryParse(Request["p"], out pageVal)
                                     ? pageVal
                                     : 1;
            ViewData["searchResults"] = _documentService.SiteSearch(Request["q"], pageNum);

            return View(page);
        }

        public ActionResult Post(SearchPage page, FormCollection form)
        {
            return Redirect(page.LiveUrlSegment + string.Format("?q={0}", form["term"]));
        }
    }
}
