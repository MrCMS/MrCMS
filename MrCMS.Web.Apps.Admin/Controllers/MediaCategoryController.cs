using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Apps.Admin.ACL;
using MrCMS.Web.Apps.Admin.Helpers;
using MrCMS.Web.Apps.Admin.ModelBinders;
using MrCMS.Web.Apps.Admin.Models;
using MrCMS.Web.Apps.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Controllers;

namespace MrCMS.Web.Apps.Admin.Controllers
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
        public RedirectToActionResult Add(AddMediaCategoryModel model)
        {
            var doc = _mediaCategoryAdminService.Add(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Show", new { id = doc.Id });
        }

        [HttpGet, ActionName("Edit")]
        public ViewResult Edit_Get(int id)
        {
            return View(_mediaCategoryAdminService.GetEditModel(id));
        }

        [HttpPost]
        public RedirectToActionResult Edit(UpdateMediaCategoryModel model)
        {
            var category = _mediaCategoryAdminService.Update(model);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", category.Name));
            return RedirectToAction("Show", new { id = category.Id });
        }

        [HttpGet, ActionName("Delete")]
        public ActionResult Delete_Get(int id)
        {
            return PartialView(_mediaCategoryAdminService.GetEditModel(id));
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var category = _mediaCategoryAdminService.Delete(id);
            TempData.InfoMessages().Add(string.Format("{0} deleted", category.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sort(int id)
        {
            List<SortItem> sortItems = _mediaCategoryAdminService.GetSortItems(id);

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort(int id, List<SortItem> items)
        {
            _mediaCategoryAdminService.SetOrders(items);
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

        ////[ChildActionOnly]
        //// TODO: refactor to view component
        //public PartialViewResult Manage(MediaCategorySearchModel searchModel)
        //{

        //    return PartialView(searchModel);
        //}


        //public ActionResult Upload(/*[IoCModelBinder(typeof(NullableEntityModelBinder))] */MediaCategory category) // TODO: model binding
        //{
        //    return PartialView(category);
        //}

        //public PartialViewResult RemoveMedia()
        //{
        //    return PartialView();
        //}

        [HttpGet]
        public ActionResult ShowFilesSimple(MediaCategory category)
        {
            return PartialView(category);
        }


        [HttpGet]
        public ActionResult SortFiles(MediaCategory parent)
        {
            ViewData["categoryId"] = parent.Id;
            List<ImageSortItem> sortItems = _fileAdminService.GetFilesToSort(parent);

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult SortFiles(int? id, List<SortItem> items)
        {
            _fileAdminService.SetOrders(items);
            return RedirectToAction("SortFiles", new { id });
        }

        /// <summary>
        ///     Finds out if the URL entered is valid.
        /// </summary>
        /// <param name="urlSegment">The URL Segment entered</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string urlSegment, int? id)
        {
            return !_mediaCategoryAdminService.UrlIsValidForMediaCategory(urlSegment, id)
                ? Json("Please choose a different Path as this one is already used.")
                : Json(true);
        }

        [Acl(typeof(MediaToolsACL), MediaToolsACL.Cut)]
        public JsonResult MoveFilesAndFolders(
            [ModelBinder(typeof(MoveFilesModelBinder))]MoveFilesAndFoldersModel model)
        {
            _fileAdminService.MoveFiles(model.Files, model.Folder);
            string message = _fileAdminService.MoveFolders(model.Folders, model.Folder);
            return Json(new FormActionResult { success = true, message = message });
        }

        [Acl(typeof(MediaToolsACL), MediaToolsACL.Delete)]
        public JsonResult DeleteFilesAndFolders(
            [ModelBinder(typeof(DeleteFilesModelBinder))]
            DeleteFilesAndFoldersModel model)
        {
            _fileAdminService.DeleteFilesSoft(model.Files);
            _fileAdminService.DeleteFoldersSoft(model.Folders);

            return Json(new FormActionResult { success = true, message = "" });
        }

        public ActionResult Directory(MediaCategorySearchModel searchModel)
        {
            //ViewData["files"] = _fileAdminService.GetFilesForFolder(searchModel);
            //ViewData["folders"] = _fileAdminService.GetSubFolders(searchModel);

            return PartialView(searchModel);
        }
    }
}