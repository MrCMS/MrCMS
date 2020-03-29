using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.ACL;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaCategoryController : MrCMSAdminController
    {
        private readonly IMediaCategoryAdminService _mediaCategoryAdminService;
        private readonly IFileAdminService _fileAdminService;

        public MediaCategoryController(IMediaCategoryAdminService mediaCategoryAdminService, IFileAdminService fileAdminService)
        {
            _mediaCategoryAdminService = mediaCategoryAdminService;
            _fileAdminService = fileAdminService;
        }

        [HttpGet, ActionName("Add")]
        public ViewResult Add_Get(int? id)
        {
            //Build list 
            var model = _mediaCategoryAdminService.GetNewCategoryModel(id);
            ViewData["parent"] = _mediaCategoryAdminService.GetCategory(id);

            return View(model);
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Add(AddMediaCategoryModel model)
        {
            var doc = await _mediaCategoryAdminService.Add(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Show", new { id = doc.Id });
        }

        [HttpGet, ActionName("Edit")]
        public ViewResult Edit_Get(int id)
        {
            return View(_mediaCategoryAdminService.GetEditModel(id));
        }

        [HttpPost]
        public async Task<RedirectToActionResult> Edit(UpdateMediaCategoryModel model)
        {
            var category = await _mediaCategoryAdminService.Update(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", category.Name));
            return RedirectToAction("Show", new { id = category.Id });
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult Delete_Get(int id)
        {
            return PartialView(_mediaCategoryAdminService.GetEditModel(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var category = await _mediaCategoryAdminService.Delete(id);
            TempData.InfoMessages().Add(string.Format("{0} deleted", category.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Sort(int id)
        {
            List<SortItem> sortItems =await _mediaCategoryAdminService.GetSortItems(id);

            return View(sortItems);
        }

        [HttpPost]
        public async Task<ActionResult> Sort(int id, List<SortItem> items)
        {
            await _mediaCategoryAdminService.SetOrders(items);
            return RedirectToAction("Sort", new { id });
        }

        public ActionResult Show(MediaCategorySearchModel searchModel)
        {
            if (searchModel == null || searchModel.Id == null)
                return RedirectToAction("Index");
            ViewData["category"] = _fileAdminService.GetCategory(searchModel);

            return View(searchModel);
        }

        public ViewResult Index(MediaCategorySearchModel searchModel)
        {
            return View(searchModel);
        }

        [HttpGet]
        public ActionResult ShowFilesSimple(MediaCategory category)
        {
            return PartialView(category);
        }


        [HttpGet]
        public ActionResult SortFiles(MediaCategory category)
        {
            ViewData["categoryId"] = category.Id;
            List<ImageSortItem> sortItems = _fileAdminService.GetFilesToSort(category);

            return View(sortItems);
        }

        [HttpPost]
        public async Task<ActionResult> SortFiles(int? id, List<SortItem> items)
        {
            await _fileAdminService.SetOrders(items);
            return RedirectToAction("SortFiles", new { id });
        }

        /// <summary>
        ///     Finds out if the URL entered is valid.
        /// </summary>
        /// <param name="urlSegment">The URL Segment entered</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult> ValidateUrlIsAllowed(string urlSegment, int? id)
        {
            return !await _mediaCategoryAdminService.UrlIsValidForMediaCategory(urlSegment, id)
                ? Json("Please choose a different Path as this one is already used.")
                : Json(true);
        }

        [Acl(typeof(MediaToolsACL), MediaToolsACL.Cut)]
        public async Task<JsonResult> MoveFilesAndFolders(
            [ModelBinder(typeof(MoveFilesModelBinder))]MoveFilesAndFoldersModel model)
        {
            await _fileAdminService.MoveFiles(model.Files, model.Folder);
            string message = await _fileAdminService.MoveFolders(model.Folders, model.Folder);
            return Json(new FormActionResult { success = true, message = message });
        }

        [Acl(typeof(MediaToolsACL), MediaToolsACL.Delete)]
        public async Task<JsonResult> DeleteFilesAndFolders(
            [ModelBinder(typeof(DeleteFilesModelBinder))]
            DeleteFilesAndFoldersModel model)
        {
            await _fileAdminService.DeleteFilesSoft(model.Files);
            await _fileAdminService.DeleteFoldersSoft(model.Folders);

            return Json(new FormActionResult { success = true, message = "" });
        }

        public ActionResult Directory(MediaCategorySearchModel searchModel)
        {
            return PartialView(searchModel);
        }
    }
}