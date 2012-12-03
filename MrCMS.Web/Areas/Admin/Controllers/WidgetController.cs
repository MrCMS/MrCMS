using System;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WidgetController : AdminController
    {
        private readonly IDocumentService _documentService;
        private readonly IWidgetService _widgetService;
        private readonly ILayoutAreaService _layoutAreaService;
        private readonly ISession _session;

        public WidgetController(IDocumentService documentService, IWidgetService widgetService, ILayoutAreaService layoutAreaService, ISession session)
        {
            _documentService = documentService;
            _widgetService = widgetService;
            _layoutAreaService = layoutAreaService;
            _session = session;
        }

        [HttpGet]
        [ValidateInput(false)]
        public ViewResultBase Add(int layoutAreaId, string returnUrl = null)
        {
            var model = _widgetService.GetAddWidgetModel(layoutAreaId, returnUrl);
            return View(model);
        }

        [HttpPost]
        [ActionName("Add")]
        [ValidateInput(false)]
        public ActionResult Add_POST(int layoutAreaId, string widgetType, string returnUrl = null, string name = "")
        {
            var newWidget = _widgetService.AddWidget(layoutAreaId, widgetType, name);

            return !string.IsNullOrWhiteSpace(returnUrl)
                       ? (ActionResult)Redirect(returnUrl)
                       : newWidget.HasProperties
                             ? RedirectToAction("Edit", "Widget", new { id = newWidget.Id })
                             : RedirectToAction("Edit", "LayoutArea", new { id = layoutAreaId });
        }

        [HttpGet]
        [ValidateInput(false)]
        public ViewResultBase Edit(int id)
        {
            var widget = _widgetService.GetWidget<Widget>(id);

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
        public ActionResult AddPageWidget(int pageId, int layoutAreaId)
        {
            var types = WidgetHelper.WidgetTypes.BuildSelectItemList(type => type.Name.BreakUpString(),
                                                                     type => type.Name,
                                                                     emptyItemText: "Select widget type...");

            return View(new AddPageWidgetModel(types, layoutAreaId, pageId, ""));
        }

        [HttpPost]
        [ActionName("AddPageWidget")]
        [ValidateInput(false)]
        public ActionResult AddPageWidget_POST(int pageId, int layoutAreaId, string widgetType, bool isRecursive, string returnUrl = null, string name = "")
        {
            Widget newWidget = WidgetHelper.GetNewWidget(widgetType);
            var webpage = _documentService.GetDocument<Webpage>(pageId);
            LayoutArea layoutArea = _layoutAreaService.GetArea(layoutAreaId);

            newWidget.LayoutArea = layoutArea;
            newWidget.Name = name;
            newWidget.Webpage = webpage;
            newWidget.IsRecursive = isRecursive;
            layoutArea.AddWidget(newWidget);

            _widgetService.SaveWidget(newWidget);
            _layoutAreaService.SaveArea(layoutArea);

            return !string.IsNullOrWhiteSpace(returnUrl)
                       ? (ActionResult)Redirect(returnUrl)
                       : newWidget.HasProperties
                             ? RedirectToAction("Edit", "Widget", new { id = newWidget.Id })
                             : RedirectToAction("Edit", "Webpage", new { id = pageId, layoutAreaId = layoutAreaId });
        }
    }
}