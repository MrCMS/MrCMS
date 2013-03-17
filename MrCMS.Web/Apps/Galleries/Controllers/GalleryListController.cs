using System.Web.Mvc;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Apps.Articles.Pages;
using MrCMS.Web.Apps.Galleries.Pages;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Galleries.Controllers
{
    public class GalleryListController : MrCMSAppUIController<GalleriesApp>
    {
        public ActionResult View(GalleryList page)
        {
            var category = Request["category"];
            var pageVal = Request["p"];
            int pageNum;
            if (!int.TryParse(pageVal, out pageNum))
            {
                pageNum = 1;
            }

            return View("~/Apps/Galleries/Views/Pages/GalleryList.cshtml", page.CurrentLayout.UrlSegment, new CategoryContainer<Gallery>(page, category, pageNum));
        }

    }
}