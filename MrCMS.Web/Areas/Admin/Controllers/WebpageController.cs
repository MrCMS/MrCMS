using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Layout;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Paging;
using MrCMS.Services;
using MrCMS.Website;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class WebpageController : BaseDocumentController<Webpage>
    {
        public WebpageController(IDocumentService documentService)
            : base(documentService)
        {
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var id = filterContext.RouteData.Values["id"];
            int idVal;
            if (!string.IsNullOrWhiteSpace(Convert.ToString(id)) && int.TryParse(Convert.ToString(id), out idVal))
            {
                var document = _documentService.GetDocument<Webpage>(idVal);
                if (document != null && !document.IsAllowedForAdmin(MrCMSApplication.CurrentUser))
                {
                    filterContext.Result = new RedirectResult("~/admin");
                }
            }
        }

        protected override void PopulateEditDropdownLists(Webpage doc)
        {
            IEnumerable<Layout> layouts = _documentService.GetAllDocuments<Layout>().Where(x => x.Hidden == false);

            ViewData["Layout"] = layouts.BuildSelectItemList(layout => layout.Name,
                                                             layout => layout.Id.ToString(CultureInfo.InvariantCulture),
                                                             layout =>
                                                             doc != null && doc.Layout != null &&
                                                             doc.Layout.Id == layout.Id,
                                                             SelectListItemHelper.EmptyItem("Default Layout"));

            var documentTypeDefinitions = (doc.Parent as Webpage).GetValidWebpageDocumentTypes().ToList();
            ViewData["DocumentTypes"] = documentTypeDefinitions;

            doc.AddTypeSpecificViewData(ViewData);
        }

        public override ActionResult View(int id)
        {
            var document = _documentService.GetDocument<Webpage>(id);

            if (document == null)
                return RedirectToAction("Index");

            return View(document);
        }

        [HttpPost]
        public ActionResult PublishNow(int id)
        {
            _documentService.PublishNow(id);

            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        public ActionResult Unpublish(int id)
        {
            _documentService.Unpublish(id);

            return RedirectToAction("Edit", new { id });
        }
        [HttpPost]
        public ActionResult HideWidget(int id, int widgetId, int layoutAreaId)
        {
            _documentService.HideWidget(id, widgetId);
            return RedirectToAction("Edit", new { id, layoutAreaId = layoutAreaId });
        }

        [HttpPost]
        public ActionResult ShowWidget(int id, int widgetId, int layoutAreaId)
        {
            _documentService.ShowWidget(id, widgetId);
            return RedirectToAction("Edit", new { id, layoutAreaId = layoutAreaId });
        }

        [HttpPost]
        public void SetParent(int id, int? parentId)
        {
            var document = _documentService.GetDocument<Webpage>(id);
            if (document == null) return;

            var existingParent = document.Parent;
            var parent = parentId.HasValue ? _documentService.GetDocument<Webpage>(parentId.Value) : null;

            document.Parent = parent;
            if (parent != null)
            {
                parent.Children.Add(document);
                _documentService.SaveDocument(parent);
            }
            if (existingParent != null)
            {
                existingParent.Children.Remove(document);
                _documentService.SaveDocument(existingParent);
            }
            _documentService.SaveDocument(document);
        }

        [HttpGet]
        public JsonResult GetForm(int id)
        {
            return Json(_documentService.GetFormStructure(id));
        }

        [HttpPost]
        public void SaveForm(int id, string data)
        {
            _documentService.SaveFormStructure(id, data);
        }

        public ActionResult ViewChanges(int id)
        {
            var documentVersion = _documentService.GetDocumentVersion(id);
            if (documentVersion == null)
                return RedirectToAction("Index");

            return PartialView(documentVersion);
        }

        [ValidateInput(false)]
        public string GetUnformattedBodyContent(int id)
        {
            var doc = _documentService.GetDocument<TextPage>(id);
            return doc.BodyContent;
        }

        [ValidateInput(false)]
        public string GetFormattedBodyContent(int id)
        {
            var doc = _documentService.GetDocument<TextPage>(id);
            MrCMSApplication.CurrentPage = doc;
            var htmlHelper = GetHtmlHelper(this);
            return htmlHelper.ParseShortcodes(doc.BodyContent).ToHtmlString();
        }

        private static HtmlHelper GetHtmlHelper(Controller controller)
        {
            var viewContext = new ViewContext(controller.ControllerContext, new FakeView(), controller.ViewData, controller.TempData, TextWriter.Null);
            return new HtmlHelper(viewContext, new ViewPage());
        }

        private class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }

        public PartialViewResult Postings(int id, int page = 1, string search = null)
        {
            var doc = _documentService.GetDocument<Webpage>(id);

            IEnumerable<FormPosting> formPostings = doc.FormPostings.OrderByDescending(posting => posting.CreatedOn);

            if (!string.IsNullOrWhiteSpace(search))
            {
                formPostings =
                    formPostings.Where(
                        posting =>
                        posting.FormValues.Any(value => value.Value.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var data = new PostingsModel(new PagedList<FormPosting>(formPostings, page, 10), id);

            return PartialView(data);
        }

        public ActionResult ViewPosting(int id)
        {
            return PartialView(_documentService.GetFormPosting(id));
        }

        public ActionResult Versions(int id, int page = 1)
        {
            var doc = _documentService.GetDocument<Webpage>(id);

            var data = new VersionsModel(new PagedList<DocumentVersion>(doc.GetVersions(), page, 10), id);

            return PartialView(data);
        }
    }
}