using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Admin.Helpers;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Web.Admin.Infrastructure.Helpers;
using MrCMS.Web.Admin.Models;
using MrCMS.Web.Admin.Services;

namespace MrCMS.Web.Admin.Controllers
{
    public class WebpageController : MrCMSAdminController
    {
        private readonly IWebpageAdminService _webpageAdminService;
        private readonly IWebpageBaseViewDataService _webpageBaseViewDataService;
        private readonly ISetWebpageAdminViewData _setWebpageAdminViewData;
        private readonly IUrlValidationService _urlValidationService;
        private readonly IModelBindingHelperAdapter _modelBindingHelperAdapter;

        public WebpageController(IWebpageAdminService webpageAdminService,
            IWebpageBaseViewDataService webpageBaseViewDataService,
            ISetWebpageAdminViewData setWebpageAdminViewData,
            IUrlValidationService urlValidationService,
            IModelBindingHelperAdapter modelBindingHelperAdapter)
        {
            _webpageAdminService = webpageAdminService;
            _webpageBaseViewDataService = webpageBaseViewDataService;
            _setWebpageAdminViewData = setWebpageAdminViewData;
            _urlValidationService = urlValidationService;
            _modelBindingHelperAdapter = modelBindingHelperAdapter;
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
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel, additionalPropertyModel.GetType(), string.Empty);
            }

            var webpage = _webpageAdminService.Add(model, additionalPropertyModel);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", webpage.Name));
            return RedirectToAction("Edit", new { id = webpage.Id });
        }

        [HttpGet]
        [ActionName("Edit")]
        public ViewResult Edit_Get(int id)
        {
            var webpage = _webpageAdminService.GetWebpage(id);
            _webpageBaseViewDataService.SetEditPageViewData(ViewData, webpage);
            _setWebpageAdminViewData.SetViewData(ViewData, webpage);
            return View(webpage);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToActionResult Edit(UpdateWebpageViewModel model)
        {
            var result = _webpageAdminService.Update(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", result.Name));
            return RedirectToAction("Edit", new { id = result.Id });
        }

        [HttpGet]
        [ActionName("Delete")]
        public ActionResult Delete_Get(int id)
        {
            return PartialView(_webpageAdminService.GetWebpage(id));
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public RedirectToActionResult Delete(int id)
        {
            var webpage = _webpageAdminService.Delete(id);
            TempData.InfoMessages().Add(string.Format("{0} deleted", webpage.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sort(int? id)
        {
            var sortItems = _webpageAdminService.GetSortItems(id);

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort(List<SortItem> items, int? id)
        {
            _webpageAdminService.SetOrders(items);
            return RedirectToAction("Sort", new { id });
        }

        //public ActionResult Show(Webpage document)
        //{
        //    if (document == null)
        //    {
        //        return RedirectToAction("Index");
        //    }

        //    return View(document);
        //}

        [HttpPost]
        public RedirectToActionResult PublishNow(int id)
        {
            _webpageAdminService.PublishNow(id);

            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        public RedirectToActionResult Unpublish(int id)
        {
            _webpageAdminService.Unpublish(id);

            return RedirectToAction("Edit", new { id });
        }

        public ActionResult ViewChanges(DocumentVersion documentVersion)
        {
            if (documentVersion == null)
            {
                return RedirectToAction("Index");
            }

            return PartialView(documentVersion);
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
        }

        [HttpGet]
        public ActionResult AddProperties(string type)
        {
            var model = _webpageAdminService.GetAdditionalPropertyModel(type);
            if (model != null)
            {
                _setWebpageAdminViewData.SetViewDataForAdd(ViewData, type);
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