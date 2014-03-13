using System.Collections.Generic;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Multisite;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Website.Binders;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaCategoryController : BaseDocumentController<MediaCategory>
    {
        private readonly IFileService _fileService;

        public MediaCategoryController(IDocumentService documentService, IFileService fileService, Site site)
            : base(documentService, site)
        {
            _fileService = fileService;
        }

        /**
         * Need to do media category specific stuff before generic stuff. In this case
         * create a directory for media files.
         */
        public override ActionResult Add(MediaCategory doc)
        {
            base.Add(doc);
            _fileService.CreateFolder(doc);
            return RedirectToAction("Show", new { id = doc.Id });
        }

        [HttpGet]
        [ActionName("Add")]
        public override ActionResult Add_Get(int? id)
        {
            //Build list 
            var model = new MediaCategory()
            {
                Parent = id.HasValue ? _documentService.GetDocument<MediaCategory>(id.Value) : null
            };

            return View(model);
        }

        [HttpPost]
        public override ActionResult Edit( MediaCategory doc)
        {
            _documentService.SaveDocument(doc);
            TempData.SuccessMessages().Add(string.Format("{0} successfully saved", doc.Name));
            return RedirectToAction("Show", new { id = doc.Id });
        }

        public ActionResult Show(MediaCategorySearchModel mediaCategorySearchModel)
        {
            if (mediaCategorySearchModel == null)
                return RedirectToAction("Index");

            var mediacategory = _documentService.GetDocument<MediaCategory>(mediaCategorySearchModel.Id);
            if (mediacategory == null)
                return RedirectToAction("Index");

            ViewData["media-category"] = mediacategory;
            return View(mediaCategorySearchModel);
        }


        public ActionResult Upload(MediaCategory category)
        {
            return PartialView(category);
        }

        public PartialViewResult MediaSelector(int? categoryId, bool imagesOnly = false, int page = 1)
        {
            ViewData["categories"] = _documentService.GetAllDocuments<MediaCategory>()
                                                     .Where(category => !category.HideInAdminNav)
                                                     .OrderBy(category => category.Name)
                                                     .BuildSelectItemList
                (category => category.Name, category => category.Id.ToString(),
                 emptyItem: SelectListItemHelper.EmptyItem("Select a category..."));
            return PartialView(_fileService.GetFilesPaged(categoryId, imagesOnly, page));
        }

        public string GetFileUrl(string value)
        {
            return _fileService.GetFileUrl(value);
        }

        public PartialViewResult MiniUploader(int id)
        {
            return PartialView(id);
        }

        public PartialViewResult FileResult(MediaFile mediaFile)
        {
            ViewData["upload"] = "upload-";
            return PartialView(mediaFile);
        }

        public PartialViewResult RemoveMedia()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult ShowFiles(MediaCategorySearchModel searchModel)
        {
            ViewData["files"] = _fileService.GetFilesForSearchPaged(searchModel);
            return PartialView(searchModel);
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
            var sortItems =
            _fileService.GetFiles(parent).OrderBy(arg => arg.display_order)
                                .Select(
                                    arg => new ImageSortItem { Order = arg.display_order, Id = arg.Id, Name = arg.name, ImageUrl = arg.url, IsImage = arg.is_image })
                                .ToList();

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult SortFiles(MediaCategory parent, List<SortItem> items)
        {
            _fileService.SetOrders(items);
            return RedirectToAction("SortFiles", new { id = parent.Id });
        }

        /// <summary>
        /// Finds out if the URL entered is valid.
        /// </summary>
        /// <param name="UrlSegment">The URL Segment entered</param>
        /// <param name="DocumentType">The type of mediaCategorySearchModel</param>
        /// <returns></returns>
        public ActionResult ValidateUrlIsAllowed(string UrlSegment, int? Id)
        {
            return !_documentService.UrlIsValidForMediaCategory(UrlSegment, Id) ? Json("Please choose a different Path as this one is already used.", JsonRequestBehavior.AllowGet) : Json(true, JsonRequestBehavior.AllowGet);
        }
    }

}