using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Website.Binders;
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
            var types = WidgetHelper.WidgetTypes.BuildSelectItemList(type => type.Name.BreakUpString(),
                                                                     type => type.Name,
                                                                     emptyItem:
                                                                         SelectListItemHelper.EmptyItem("Select widget type..."));

            return View(new AddWidgetModel(types, layoutAreaId, returnUrl, ""));
        }

        [HttpPost]
        [ActionName("Add")]
        [ValidateInput(false)]
        public ActionResult Add_POST(int layoutAreaId, string widgetType, string returnUrl = null, string name = "")
        {
            Widget newWidget = WidgetHelper.GetNewWidget(widgetType);
            LayoutArea layoutArea = _layoutAreaService.GetArea(layoutAreaId);

            newWidget.LayoutArea = layoutArea;
            newWidget.Name = name;
            layoutArea.AddWidget(newWidget);

            _widgetService.SaveWidget(newWidget);
            _layoutAreaService.SaveArea(layoutArea);

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
        public ActionResult Delete_Get(int id)
        {
            var widget = _widgetService.GetWidget<Widget>(id);
            return PartialView(widget);
        }

        [HttpPost]
        public ActionResult Delete(int id, string returnUrl)
        {
            var widget = _widgetService.GetWidget<Widget>(id);
            var webpageId = 0;
            if (widget.Webpage != null)
                webpageId = widget.Webpage.Id;
            var layoutAreaId = widget.LayoutArea.Id;
            _widgetService.DeleteWidget(id);

            return !string.IsNullOrWhiteSpace(returnUrl) ? (ActionResult) Redirect(returnUrl) :
                webpageId > 0
                       ? RedirectToAction("Edit", "Webpage", new { id = webpageId, layoutAreaId = layoutAreaId })
                       : RedirectToAction("Edit", "LayoutArea", new { id = layoutAreaId });
        }

        [HttpGet]
        [ValidateInput(false)]
        public ActionResult AddPageWidget(int pageId, int layoutAreaId)
        {
            var types = WidgetHelper.WidgetTypes.BuildSelectItemList(type => type.Name.BreakUpString(),
                                                                     type => type.Name,
                                                                     emptyItem: SelectListItemHelper.
                                                                         EmptyItem("Select widget type..."));

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

    public class AddPageWidgetModel
    {
        private readonly IEnumerable<SelectListItem> _types;
        private readonly int _layoutAreaId;
        private readonly int _pageId;
        private readonly string _name;
        private bool _isRecursive = true;

        public AddPageWidgetModel(IEnumerable<SelectListItem> types, int layoutAreaId, int pageId, string name)
        {
            _types = types;
            _layoutAreaId = layoutAreaId;
            _pageId = pageId;
            _name = name;
        }

        public IEnumerable<SelectListItem> Types
        {
            get { return _types; }
        }

        public int LayoutAreaId
        {
            get { return _layoutAreaId; }
        }

        public int PageId
        {
            get { return _pageId; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsRecursive
        {
            get { return _isRecursive; }
        }
    }

    public class AddWidgetModel
    {
        private readonly IEnumerable<SelectListItem> _types;
        private readonly int _layoutAreaId;
        private readonly string _returnUrl;
        private readonly string _name;
        private bool _isRecursive = true;

        public AddWidgetModel(IEnumerable<SelectListItem> types, int layoutAreaId, string returnUrl, string name)
        {
            _types = types;
            _layoutAreaId = layoutAreaId;
            _returnUrl = returnUrl;
            _name = name;
        }

        public IEnumerable<SelectListItem> Types
        {
            get { return _types; }
        }

        public int LayoutAreaId
        {
            get { return _layoutAreaId; }
        }

        public string ReturnUrl
        {
            get { return _returnUrl; }
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsRecursive
        {
            get { return _isRecursive; }
        }
    }
}