using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Models;
using MrCMS.Services;
using MrCMS.Web.Areas.Admin.Models;
using MrCMS.Web.Areas.Admin.Services;

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
            return RedirectToAction("Show", new {id = doc.Id});
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
            return RedirectToAction("Show", new {id = doc.Id});
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

        public PartialViewResult RemoveMedia()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult ShowFiles(MediaCategorySearchModel searchModel)
        {
            ViewData["files"] = _fileAdminService.GetFilesForSearchPaged(searchModel);
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
            List<ImageSortItem> sortItems =
                _fileAdminService.GetFiles(parent).OrderBy(arg => arg.display_order)
                    .Select(
                        arg =>
                            new ImageSortItem
                            {
                                Order = arg.display_order,
                                Id = arg.Id,
                                Name = arg.name,
                                ImageUrl = arg.url,
                                IsImage = arg.is_image
                            })
                    .ToList();

            return View(sortItems);
        }

        [HttpPost]
        public ActionResult SortFiles(MediaCategory parent, List<SortItem> items)
        {
            _fileAdminService.SetOrders(items);
            return RedirectToAction("SortFiles", new {id = parent.Id});
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
    }
}