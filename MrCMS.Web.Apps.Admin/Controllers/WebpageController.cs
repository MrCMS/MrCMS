using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.ACL;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;

namespace MrCMS.Web.Apps.Admin.Controllers
{
    public class WebpageController : MrCMSAdminController
    {
        private readonly IWebpageAdminService _webpageAdminService;
        private readonly IWebpageBaseViewDataService _webpageBaseViewDataService;
        private readonly IUrlValidationService _urlValidationService;

        public WebpageController(IWebpageAdminService webpageAdminService,
            IWebpageBaseViewDataService webpageBaseViewDataService, IUrlValidationService urlValidationService)
        {
            _webpageAdminService = webpageAdminService;
            _webpageBaseViewDataService = webpageBaseViewDataService;
            _urlValidationService = urlValidationService;
        }

        public ViewResult Index()
        {
            return View();
        }


        [HttpGet]
        [ActionName("Add")]
        public ViewResult Add_Get(int? id)
        {
            //Build list 
            var model = _webpageAdminService.GetAddModel(id);

            Webpage parent = _webpageAdminService.GetWebpage(id);
            _webpageBaseViewDataService.SetAddPageViewData(ViewData, parent);

            return View(model);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public async Task<ActionResult> Add(AddWebpageModel model)
        {
            if (!_urlValidationService.UrlIsValidForWebpage(model.UrlSegment, null))
            {
                Webpage parent = _webpageAdminService.GetWebpage(model.ParentId);
                _webpageBaseViewDataService.SetAddPageViewData(ViewData, parent);
                return View(model);
            }

            var additionalPropertyModel = _webpageAdminService.GetAdditionalPropertyModel(model.DocumentType);
            if (additionalPropertyModel != null)
                await TryUpdateModelAsync(additionalPropertyModel, additionalPropertyModel.GetType(), string.Empty);
            var webpage = _webpageAdminService.Add(model, additionalPropertyModel);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", webpage.Name));
            return RedirectToAction("Edit", new { id = webpage.Id });
        }

        [HttpGet]
        [ActionName("Edit")]
        [Acl(typeof(Webpage), TypeACLRule.Edit)]
        public ActionResult Edit_Get(int id)
        {
            var webpage = _webpageAdminService.GetWebpage(id);
            _webpageBaseViewDataService.SetEditPageViewData(ViewData, webpage);
            webpage.SetAdminViewData(this);
            return View(webpage);
        }

        [HttpPost]
        [Acl(typeof(Webpage), TypeACLRule.Edit)]
        [ForceImmediateLuceneUpdate]
        public ActionResult Edit(UpdateWebpageViewModel model)
        {
            var result = _webpageAdminService.Update(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", result.Name));
            return RedirectToAction("Edit", new { id = result.Id });
        }

        [HttpGet]
        [Acl(typeof(Webpage), TypeACLRule.Delete)]
        [ActionName("Delete")]
        public ActionResult Delete_Get(int id)
        {
            return PartialView(_webpageAdminService.GetWebpage(id));
        }

        [HttpPost]
        [Acl(typeof(Webpage), TypeACLRule.Delete)]
        [ForceImmediateLuceneUpdate]
        public ActionResult Delete(int id)
        {
            var webpage = _webpageAdminService.Delete(id);
            TempData.InfoMessages().Add(string.Format("{0} deleted", webpage.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sort(
            int? id) // TODO: model-binding
        {
            var sortItems = _webpageAdminService.GetSortItems(id);

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort(
            //[IoCModelBinder(typeof(NullableEntityModelBinder))]
            Webpage parent, // TODO: model-binding
            List<SortItem> items)
        {
            _webpageAdminService.SetOrders(items);
            return RedirectToAction("Sort", parent == null ? null : new { id = parent.Id });
        }

        public ActionResult Show(Webpage document)
        {
            if (document == null)
                return RedirectToAction("Index");

            return View(document);
        }

        [HttpPost]
        public ActionResult PublishNow(int id)
        {
            _webpageAdminService.PublishNow(id);

            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        public ActionResult Unpublish(int id)
        {
            _webpageAdminService.Unpublish(id);

            return RedirectToAction("Edit", new { id });
        }

        public ActionResult ViewChanges(DocumentVersion documentVersion)
        {
            if (documentVersion == null)
                return RedirectToAction("Index");

            return PartialView(documentVersion);
        }

        [HttpGet]
        public PartialViewResult FormProperties(Webpage webpage)
        {
            return PartialView(webpage);
        }

        /// <summary>
        ///     Finds out if the URL entered is valid for a webpage
        /// </summary>
        /// <param name="urlSegment">The URL Segment entered</param>
        /// <param name="id">The Id of the current document if it is set</param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string urlSegment, int? id)
        {
            return !_urlValidationService.UrlIsValidForWebpage(urlSegment, id)
                ? Json("Please choose a different URL as this one is already used.")
                : Json(true);
        }

        /// <summary>
        ///     Returns server date used for publishing (can't use JS date as can be out compared to server date)
        /// </summary>
        /// <returns>Date</returns>
        public string GetServerDate()
        {
            return _webpageAdminService.GetServerDate();
            //return DateTime.Now.ToString(); // TODO: sort out dates and cultures
            //return CurrentRequestData.Now.ToString(CurrentRequestData.CultureInfo);
        }

        [HttpGet]
        public ActionResult AddProperties(string type)
        {
            var model = _webpageAdminService.GetAdditionalPropertyModel(type);
            if (model != null)
            {
                // TODO: viewdata
                ViewData["type"] = TypeHelper.GetTypeByName(type);
                return PartialView(model);
            }
            return new EmptyResult();
        }

        public PartialViewResult Versions(Webpage webpage, int page = 1)
        {
            ViewData["page"] = page;
            return PartialView(webpage);
        }
    }
}