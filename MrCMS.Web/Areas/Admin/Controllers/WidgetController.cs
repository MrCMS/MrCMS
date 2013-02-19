using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Website.Binders;
using MrCMS.Website.Controllers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WidgetController : MrCMSAdminController
    {
        private readonly IDocumentService _documentService;
        private readonly IWidgetService _widgetService;
        private readonly ISession _session;

        public WidgetController(IDocumentService documentService, IWidgetService widgetService, ISession session)
        {
            _documentService = documentService;
            _widgetService = widgetService;
            _session = session;
        }

        [HttpGet]
        [ValidateInput(false)]
        public PartialViewResult Add(LayoutArea layoutArea, string returnUrl = null)
        {
            TempData["returnUrl"] = returnUrl;
            var model = new AddWidgetModel
                {
                    LayoutArea = layoutArea
                };
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Add")]
        [ValidateInput(false)]
        public ActionResult Add_POST([IoCModelBinder(typeof(AddWidgetModelBinder))] Widget widget, string returnUrl = null)
        {
            var newWidget = _widgetService.AddWidget(widget);

            return !string.IsNullOrWhiteSpace(returnUrl)
                       ? (ActionResult)Redirect(returnUrl)
                       : newWidget.HasProperties
                             ? RedirectToAction("Edit", "Widget", new { id = newWidget.Id })
                             : RedirectToAction("Edit", "LayoutArea", new { id = newWidget.LayoutArea.Id });
        }

        [HttpGet]
        [ValidateInput(false)]
        [ActionName("Edit")]
        public ViewResultBase Edit_Get(Widget widget)
        {
            widget.SetDropdownData(ViewData, _session);

            return View(widget);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Widget widget,
                                 string returnUrl = null)
        {
            _widgetService.SaveWidget(widget);

            return string.IsNullOrWhiteSpace(returnUrl)
                       ? widget.Webpage != null
                             ? RedirectToAction("Edit", "Webpage", new { id = widget.Webpage.Id })
                             : (ActionResult)RedirectToAction("Edit", "LayoutArea", new { id = widget.LayoutArea.Id })
                       : Redirect(returnUrl);
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(Widget widget)
        {
            return PartialView(widget);
        }

        [HttpPost]
        public ActionResult Delete(Widget widget, string returnUrl)
        {
            var webpageId = 0;
            var layoutAreaId = 0;
            if (widget.Webpage != null)
                webpageId = widget.Webpage.Id;
            if (widget.LayoutArea != null)
                layoutAreaId = widget.LayoutArea.Id;
            _widgetService.DeleteWidget(widget);

            return !string.IsNullOrWhiteSpace(returnUrl) &&
                   !returnUrl.Contains("widget/edit/", StringComparison.OrdinalIgnoreCase)
                       ? (ActionResult)Redirect(returnUrl)
                       : webpageId > 0
                             ? RedirectToAction("Edit", "Webpage", new { id = webpageId, layoutAreaId })
                             : RedirectToAction("Edit", "LayoutArea", new { id = layoutAreaId });
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult AddPageWidget(LayoutArea layoutArea, int pageId, string returnUrl = null)
        {
            TempData["returnUrl"] = returnUrl;
            var model = new AddWidgetModel
            {
                LayoutArea = layoutArea,
                Webpage = _documentService.GetDocument<Webpage>(pageId)
            };
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("AddPageWidget")]
        [ValidateInput(false)]
        public ActionResult AddPageWidget_POST([IoCModelBinder(typeof(AddWidgetModelBinder))] Widget widget, string returnUrl = null)
        {
            _widgetService.SaveWidget(widget);

            return !string.IsNullOrWhiteSpace(returnUrl)
                       ? (ActionResult)Redirect(returnUrl)
                       : widget.HasProperties
                             ? RedirectToAction("Edit", "Widget", new { id = widget.Id })
                             : RedirectToAction("Edit", "Webpage", new { id = widget.Webpage.Id, layoutAreaId = widget.LayoutArea.Id });
        }
    }
}