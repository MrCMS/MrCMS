using System;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL;
using MrCMS.Data;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Widget;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class WidgetController : MrCMSAdminController
    {
        private readonly IRepository<Webpage> _webpageRepository;
        private readonly IWidgetService _widgetService;

        public WidgetController(IRepository<Webpage> webpageRepository, IWidgetService widgetService)
        {
            _webpageRepository = webpageRepository;
            _widgetService = widgetService;
        }

        [HttpGet]
        public PartialViewResult Add(LayoutArea layoutArea, int pageId = 0, string returnUrl = null)
        {
            TempData["returnUrl"] = returnUrl;
            var model = new AddWidgetModel
            {
                LayoutArea = layoutArea,
                Webpage = _webpageRepository.Get(pageId)
            };
            return PartialView(model);
        }

        [HttpPost]
        [ActionName("Add")]
        public JsonResult Add_POST(
            //[IoCModelBinder(typeof (AddWidgetModelBinder))]
            Widget widget) // TODO: model-binding
        {
            _widgetService.AddWidget(widget);

            return Json(widget.Id);
        }

        [HttpGet]
        //[ValidateInput(false)]
        [ActionName("Edit")]
        [Acl(typeof (Widget), TypeACLRule.Edit)]
        public ViewResult Edit_Get(Widget widget, string returnUrl = null)
        {
            widget.SetViewData(this);

            if (!string.IsNullOrEmpty(returnUrl))
                ViewData["return-url"] = Request.Referer();
            else
                ViewData["return-url"] = returnUrl;

            return View(widget);
        }

        [HttpPost]
        [Acl(typeof (Widget), TypeACLRule.Edit)]
        public ActionResult Edit(Widget widget,
            string returnUrl = null)
        {
            _widgetService.SaveWidget(widget);

            return string.IsNullOrWhiteSpace(returnUrl)
                ? widget.Webpage != null
                    ? RedirectToAction("Edit", "Webpage", new {id = widget.Webpage.Id})
                    : (ActionResult) RedirectToAction("Edit", "LayoutArea", new {id = widget.LayoutArea.Id})
                : Redirect(returnUrl);
        }

        [HttpGet]
        [ActionName("Delete")]
        [Acl(typeof (Widget), TypeACLRule.Delete)]
        public ActionResult Delete_Get(Widget widget)
        {
            return PartialView(widget);
        }

        [HttpPost]
        [Acl(typeof (Widget), TypeACLRule.Delete)]
        public ActionResult Delete(Widget widget, string returnUrl)
        {
            int webpageId = 0;
            int layoutAreaId = 0;
            if (widget.Webpage != null)
                webpageId = widget.Webpage.Id;
            if (widget.LayoutArea != null)
                layoutAreaId = widget.LayoutArea.Id;
            _widgetService.DeleteWidget(widget);

            return !string.IsNullOrWhiteSpace(returnUrl) &&
                   !returnUrl.Contains("widget/edit/", StringComparison.OrdinalIgnoreCase)
                ? (ActionResult) Redirect(returnUrl)
                : webpageId > 0
                    ? RedirectToAction("Edit", "Webpage", new {id = webpageId, layoutAreaId})
                    : RedirectToAction("Edit", "LayoutArea", new {id = layoutAreaId});
        }

        [HttpGet]
        public ActionResult AddProperties(
            //[IoCModelBinder(typeof(AddWidgetPropertiesModelBinder))]
            Widget widget) // TODO: model-binding
        {
            if (widget != null)
            {
                widget.SetViewData(this);
                return PartialView(widget);
            }
            return new EmptyResult();
        }
    }
}