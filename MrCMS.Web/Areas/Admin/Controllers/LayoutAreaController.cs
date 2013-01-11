using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Binders;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class LayoutAreaController : AdminController
    {
        private readonly IDocumentService _documentService;
        private readonly ILayoutAreaService _layoutAreaService;

        public LayoutAreaController(ILayoutAreaService layoutAreaService, IDocumentService documentService)
        {
            _layoutAreaService = layoutAreaService;
            _documentService = documentService;
        }

        [HttpGet]
        public PartialViewResult Add(int id)
        {
            var layout = _documentService.GetDocument<Layout>(id);
            return PartialView("Add", new LayoutArea { Layout = layout });
        }

        [HttpPost]
        public ActionResult Add([SessionModelBinder(typeof(AddLayoutAreaModelBinder))] LayoutArea layoutArea)
        {
            _layoutAreaService.SaveArea(layoutArea);

            return RedirectToAction("Edit", "Layout", new { id = layoutArea.Layout.Id });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            LayoutArea layoutArea = _layoutAreaService.GetArea(id);

            if (layoutArea == null)
                return RedirectToAction("Index", "Layout");

            return View(layoutArea);
        }

        [HttpPost]
        public ActionResult Edit([SessionModelBinder(typeof(LayoutAreaModelBinder))] LayoutArea area)
        {
            _layoutAreaService.SaveArea(area);

            return RedirectToAction("Edit", "Layout", new { id = area.Layout.Id });
        }


        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(LayoutArea area)
        {
            return PartialView(area);
        }

        [HttpPost]
        public ActionResult Delete(LayoutArea area)
        {
            var layoutId = area.Layout.Id;
            _layoutAreaService.DeleteArea(area);

            return RedirectToAction("Edit", "Layout", new { id = layoutId });
        }

        [HttpGet]
        public ActionResult SortWidgets(int id)
        {
            var area = _layoutAreaService.GetArea(id);

            return View(area.GetWidgets());
        }

        public void SortWidgetsAction(string orders)
        {
            _layoutAreaService.SetWidgetOrders(orders);
        }

        [HttpGet]
        public ActionResult SortWidgetsForPage(int id, int pageId)
        {
            var area = _layoutAreaService.GetArea(id);
            var webpage = _documentService.GetDocument<Webpage>(pageId);

            return View(new PageWidgetSortModel(area.GetWidgets(webpage), webpage, area));
        }

        [HttpPost]
        public ActionResult SortWidgetsForPage(PageWidgetSortModel pageWidgetSortModel)
        {
            _layoutAreaService.SetWidgetForPageOrders(pageWidgetSortModel);
            return RedirectToAction("Edit", "Webpage", new {id = pageWidgetSortModel.WebpageId});
        }

        [HttpPost]
        public ActionResult ResetSorting(int id, int pageId)
        {
            var area = _layoutAreaService.GetArea(id);
            var webpage = _documentService.GetDocument<Webpage>(pageId);
            _layoutAreaService.ResetSorting(area, webpage);

            return RedirectToAction("Edit", "Webpage", new {id = pageId});
        }
    }
}