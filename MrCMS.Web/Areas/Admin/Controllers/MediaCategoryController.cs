using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.ACL;
using MrCMS.Web.Areas.Admin.ModelBinders;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;
using MrCMS.Website;
using MrCMS.Website.Binders;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaCategoryController : BaseDocumentController<MediaCategory>
    {
        private readonly IFileAdminService _fileAdminService;

        public MediaCategoryController(IFileAdminService fileAdminService, IDocumentService documentService,
            IUrlValidationService urlValidationService)
            : base(documentService, urlValidationService)
        {
            _fileAdminService = fileAdminService;
        }

        /**
         * Need to do media category specific stuff before generic stuff. In this case
         * create a directory for media files.
         */

        public override ActionResult Add(MediaCategory doc)
        {
            base.Add(doc);
            _fileAdminService.CreateFolder(doc);
            return RedirectToAction("Show", new { id = doc.Id });
        }

        [HttpGet]
        [ActionName("Add")]
        public override ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new MediaCategory
            {
                Parent = id.HasValue ? _documentService.GetDocument<MediaCategory>(id.Value) : null
            };

            return View(model);
        }

        [HttpPost]
        public override ActionResult Edit(MediaCategory doc)
        {
            _documentService.SaveDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Show", new { id = doc.Id });
        }

        public ActionResult Show(MediaCategory mediaCategory)
        {
            if (mediaCategory == null)
                return RedirectToAction("Index");
            ViewData["files"] = _fileAdminService.GetFilesForFolder(mediaCategory);
            ViewData["folders"] = _fileAdminService.GetSubFolders(mediaCategory);

            return View(mediaCategory);
        }

        public override ViewResult Index()
        {
            ViewData["files"] = _fileAdminService.GetFilesForFolder(null);
            ViewData["folders"] = _fileAdminService.GetSubFolders(null);

            return View();
        }


        public ActionResult Upload([IoCModelBinder(typeof(NullableEntityModelBinder))]MediaCategory category)
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
            List<ImageSortItem> sortItems =
                _fileAdminService.GetFilesForFolder(parent).OrderBy(arg => arg.DisplayOrder)
                    .Select(
                        arg =>
                            new ImageSortItem
                            {
                                Order = arg.DisplayOrder,
                                Id = arg.Id,
                                Name = arg.FileName,
                                ImageUrl = arg.FileUrl,
                                IsImage = arg.IsImage
                            })
                    .ToList();

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
        /// <param name="UrlSegment">The URL Segment entered</param>
        /// <param name="DocumentType">The type of mediaCategorySearchModel</param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string UrlSegment, int? Id)
        {
            return !_urlValidationService.UrlIsValidForMediaCategory(UrlSegment, Id)
                ? Json("Please choose a different Path as this one is already used.", JsonRequestBehavior.AllowGet)
                : Json(true, JsonRequestBehavior.AllowGet);
        }

        [MrCMSACLRule(typeof(MediaToolsACL), MediaToolsACL.Cut)]
        public JsonResult MoveFilesAndFolders(
            [IoCModelBinder(typeof(MoveFilesModelBinder))] MoveFilesAndFoldersModel model)
        {
            _fileAdminService.MoveFiles(model.Files, model.Folder);
            string message = _fileAdminService.MoveFolders(model.Folders, model.Folder);
            return Json(new FormActionResult { success = true, message = message});
        }

        [MrCMSACLRule(typeof(MediaToolsACL), MediaToolsACL.Delete)]
        public JsonResult DeleteFilesAndFolders(
            [IoCModelBinder(typeof(DeleteFilesModelBinder))] DeleteFilesAndFoldersModel model)
        {
            _fileAdminService.DeleteFilesSoft(model.Files);
            _fileAdminService.DeleteFoldersSoft(model.Folders);

            return Json(new FormActionResult { success = true, message = "" });
        }


    }
}