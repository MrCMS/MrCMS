using System;
using System.Drawing;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Website.Binders;
using System.Linq;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaCategoryController : BaseDocumentController<MediaCategory>
    {
        private readonly IFileService _fileService;
        private readonly IImageProcessor _imageProcessor;

        public MediaCategoryController(IDocumentService documentService, IFileService fileService, IImageProcessor imageProcessor)
            : base(documentService)
        {
            _fileService = fileService;
            _imageProcessor = imageProcessor;
        }

        /**
         * Need to do media category specific stuff before generic stuff. In this case
         * create a directory for media files.
         */

        public override ActionResult Add([SessionModelBinder(typeof(AddDocumentModelBinder))] MediaCategory doc)
        {
            return base.Add(doc);
        }

        public override ActionResult Show(MediaCategory document)
        {
            if (document != null)
                return RedirectToAction("Edit", new { document.Id });
            return RedirectToAction("Index");
        }

        public ActionResult Upload(MediaCategory category)
        {
            return PartialView(category);
        }

        public ActionResult UploadTemplate()
        {
            return PartialView();
        }

        public ActionResult DownloadTemplate()
        {
            return PartialView();
        }

        public ActionResult Thumbnails()
        {
            return PartialView();
        }

        public ActionResult MediaSelector(int? categoryId, bool imagesOnly = false, int page = 1)
        {
            ViewData["categories"] = _documentService.GetAllDocuments<MediaCategory>().OrderBy(category => category.Name).BuildSelectItemList
                       (category => category.Name, category => category.Id.ToString(),
                        emptyItem: SelectListItemHelper.EmptyItem("Select a category..."));
            return View(_fileService.GetFilesPaged(categoryId, imagesOnly, page));
        }

        public PartialViewResult Sizes()
        {
            var items = _imageProcessor.GetImageSizes();
            return PartialView(items);
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
            return PartialView(mediaFile);
        }

        public PartialViewResult RemoveMedia()
        {
            return PartialView();
        }
    }
}