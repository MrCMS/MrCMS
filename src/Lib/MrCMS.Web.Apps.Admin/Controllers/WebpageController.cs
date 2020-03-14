using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website.Controllers;
using MrCMS.Website.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public async Task<ViewResult> Add_Get(int? id)
        {
            //Build list 
            var model = _webpageAdminService.GetAddModel(id);

            Webpage parent = await _webpageAdminService.GetWebpage(id);
            await _webpageBaseViewDataService.SetAddPageViewData(ViewData, parent);

            return View(model);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public async Task<ActionResult> Add(AddWebpageModel model)
        {
            if (!await _urlValidationService.UrlIsValidForWebpage(model.UrlSegment, null))
            {
                Webpage parent = await _webpageAdminService.GetWebpage(model.ParentId);
                await _webpageBaseViewDataService.SetAddPageViewData(ViewData, parent);
                return View(model);
            }

            var additionalPropertyModel = _webpageAdminService.GetAdditionalPropertyModel(model.DocumentType);
            if (additionalPropertyModel != null)
            {
                await _modelBindingHelperAdapter.TryUpdateModelAsync(this, additionalPropertyModel, additionalPropertyModel.GetType(), string.Empty);
            }

            var webpage = await _webpageAdminService.Add(model, additionalPropertyModel);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", webpage.Name));
            return RedirectToAction("Edit", new { id = webpage.Id });
        }

        [HttpGet]
        [ActionName("Edit")]
        public async Task<ViewResult> Edit_Get(int id)
        {
            var webpage = await _webpageAdminService.GetWebpage(id);
            await _webpageBaseViewDataService.SetEditPageViewData(ViewData, webpage);
            await _setWebpageAdminViewData.SetViewData(ViewData, webpage);
            return View(webpage);
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public async Task<RedirectToActionResult> Edit(UpdateWebpageViewModel model)
        {
            var result = await _webpageAdminService.Update(model);
            if (result.Success)
                TempData.SuccessMessages().Add(string.Format("{0} successfully saved", result.Model.Name));
            else
                TempData.ErrorMessages().Add("An error occurred: " + string.Join(", ", result.Messages));
            return RedirectToAction("Edit", new { id = model.Id });
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<ActionResult> Delete_Get(int id)
        {
            return PartialView(await _webpageAdminService.GetWebpage(id));
        }

        [HttpPost]
        [ForceImmediateLuceneUpdate]
        public async Task<RedirectToActionResult> Delete(int id)
        {
            var webpage = await _webpageAdminService.Delete(id);
            TempData.InfoMessages().Add(string.Format("{0} deleted", webpage.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Sort(int? id)
        {
            var sortItems = await _webpageAdminService.GetSortItems(id);

            return View(sortItems);
        }

        [HttpPost]
        public async Task<ActionResult> Sort(List<SortItem> items, int? id)
        {
            await _webpageAdminService.SetOrders(items);
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
        public async Task<RedirectToActionResult> PublishNow(int id)
        {
            await _webpageAdminService.PublishNow(id);

            return RedirectToAction("Edit", new { id });
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Unpublish(int id)
        {
            await _webpageAdminService.Unpublish(id);

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
        public async Task<ActionResult> ValidateUrlIsAllowed(string urlSegment, int? id)
        {
            return !await _urlValidationService.UrlIsValidForWebpage(urlSegment, id)
                ? Json("Please choose a different URL as this one is already used.")
                : Json(true);
        }

        /// <summary>
        ///     Returns server date used for publishing (can't use JS date as can be out compared to server date)
        /// </summary>
        /// <returns>Date</returns>
        public Task<string> GetServerDate()
        {
            return _webpageAdminService.GetServerDate();
        }

        [HttpGet]
        public async Task<ActionResult> AddProperties(string type)
        {
            var model = _webpageAdminService.GetAdditionalPropertyModel(type);
            if (model != null)
            {
                await _setWebpageAdminViewData.SetViewDataForAdd(ViewData, type);
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