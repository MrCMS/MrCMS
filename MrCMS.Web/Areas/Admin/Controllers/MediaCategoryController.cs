using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Models;
using MrCMS.Web.Areas.Admin.ACL;
using MrCMS.Web.Areas.Admin.Helpers;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;
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

            return View(model);
        }

        [HttpPost]
        public  ActionResult Add(MediaCategory doc)
        {
            _mediaCategoryAdminService.Add(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully added", doc.Name));
            return RedirectToAction("Show", new { id = doc.Id });
        }

        [HttpGet, ActionName("Edit")]
        public  ActionResult Edit_Get(MediaCategory doc)
        {
            return View(doc);
        }

        [HttpPost]
        public  ActionResult Edit(MediaCategory doc)
        {
            _mediaCategoryAdminService.Update(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Show", new { id = doc.Id });
        }

        [HttpGet, ActionName("Delete")]
        public  ActionResult Delete_Get(MediaCategory document)
        {
            return PartialView(document);
        }

        [HttpPost]
        public  ActionResult Delete(MediaCategory document)
        {
            _mediaCategoryAdminService.Delete(document);
            TempData.InfoMessages().Add(string.Format("{0} deleted", document.Name));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))] MediaCategory parent)
        {
            List<SortItem> sortItems = _mediaCategoryAdminService.GetSortItems(parent);

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult Sort([IoCModelBinder(typeof(NullableEntityModelBinder))] MediaCategory parent, List<SortItem> items)
        {
            _mediaCategoryAdminService.SetOrders(items);
            return RedirectToAction("Sort", parent == null ? null : new { id = parent.Id });
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

        [ChildActionOnly]
        public PartialViewResult Manage(MediaCategorySearchModel searchModel)
        {
            ViewData["category"] = _fileAdminService.GetCategory(searchModel);
            ViewData["sort-by-options"] = _fileAdminService.GetSortByOptions(searchModel);

            return PartialView(searchModel);
        }


        public ActionResult Upload([IoCModelBinder(typeof(NullableEntityModelBinder))] MediaCategory category)
        {
            return PartialView(category);
        }

        public PartialViewResult RemoveMedia()
        {
            return PartialView();
        }

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
        public ActionResult SortFiles(MediaCategory parent, List<SortItem> items)
        {
            _fileAdminService.SetOrders(items);
            return RedirectToAction("SortFiles", new { id = parent.Id });
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
                ? Json("Please choose a different Path as this one is already used.", JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }

        [MrCMSACLRule(typeof(MediaToolsACL), MediaToolsACL.Cut)]
        public JsonResult MoveFilesAndFolders(
            [IoCModelBinder(typeof(MoveFilesModelBinder))] MoveFilesAndFoldersModel model)
        {
            _fileAdminService.MoveFiles(model.Files, model.Folder);
            string message = _fileAdminService.MoveFolders(model.Folders, model.Folder);
            return Json(new FormActionResult { success = true, message = message });
        }

        [MrCMSACLRule(typeof(MediaToolsACL), MediaToolsACL.Delete)]
        public JsonResult DeleteFilesAndFolders(
            [IoCModelBinder(typeof(DeleteFilesModelBinder))] DeleteFilesAndFoldersModel model)
        {
            _fileAdminService.DeleteFilesSoft(model.Files);
            _fileAdminService.DeleteFoldersSoft(model.Folders);

            return Json(new FormActionResult { success = true, message = "" });
        }

        public ActionResult Directory(MediaCategorySearchModel searchModel)
        {
            ViewData["files"] = _fileAdminService.GetFilesForFolder(searchModel);
            ViewData["folders"] = _fileAdminService.GetSubFolders(searchModel);

            return PartialView(searchModel);
        }
    }
}