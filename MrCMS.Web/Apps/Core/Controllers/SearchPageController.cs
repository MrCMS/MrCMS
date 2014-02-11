using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MrCMS.Models.Search;
using MrCMS.Services;
using MrCMS.Services.Search;
using MrCMS.Web.Apps.Core.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Core.Controllers
{
    public class SearchPageController : MrCMSAppUIController<CoreApp>
    {
        private readonly IWebpageSearchService _webpageSearchService;

        public SearchPageController(IWebpageSearchService webpageSearchService)
        {
            _webpageSearchService = webpageSearchService;
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
            ViewData["searchResults"] = _webpageSearchService.Search(new AdminWebpageSearchQuery
            {
                Term = Request["q"],
                Page = pageNum
            });

            return View(page);
        }

        public ActionResult Post(SearchPage page, FormCollection form)
        {
            return Redirect(page.LiveUrlSegment + string.Format("?q={0}", form["term"]));
        }
    }
}
