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

        public override ActionResult View(int id)
        {
            if (_documentService.GetDocument<MediaCategory>(id) != null)
                return RedirectToAction("Edit", new { id });
            return RedirectToAction("Index");
        }

        public ActionResult Upload(int id)
        {
            var category = _documentService.GetDocument<MediaCategory>(id);
            return PartialView(category);
        }

        public ActionResult Media(MediaCategory category, string name)
        {
            if (category == null)
            {
                if (name.Length > 0)
                    category = new MediaCategory { Name = name };
            }

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

        public ActionResult ImageSelector()
        {
            var categories =
                _documentService.GetAllDocuments<MediaCategory>().OrderBy(category => category.Name).BuildSelectItemList
                    (category => category.Name, category => category.Id.ToString(),
                     emptyItem: SelectListItemHelper.EmptyItem("Select a category..."));
            return View(categories);
        }

        public ActionResult FileSelector()
        {
            var categories =
                _documentService.GetAllDocuments<MediaCategory>().OrderBy(category => category.Name).BuildSelectItemList
                    (category => category.Name, category => category.Id.ToString(),
                     emptyItem: SelectListItemHelper.EmptyItem("Select a category..."));
            return PartialView(categories);
        }

        public PartialViewResult ImageList(int id)
        {
            var mediaCategory = _documentService.GetDocument<MediaCategory>(id);
            var mediaFiles = mediaCategory.Files.Where(file => file.IsImage);
            var items = mediaFiles.BuildSelectItemList(file => file.FileName, file => file.Id.ToString(),
                                                       emptyItem: SelectListItemHelper.EmptyItem("Select an image..."));
            return PartialView(items);
        }

        public PartialViewResult FileList(int id)
        {
            var mediaCategory = _documentService.GetDocument<MediaCategory>(id);
            var mediaFiles = mediaCategory.Files.Where(file => !file.IsImage);
            var items = mediaFiles.BuildSelectItemList(file => file.FileName, file => file.Id.ToString(),
                                                       emptyItem: SelectListItemHelper.EmptyItem("Select a file..."));
            return PartialView(items);
        }

        public PartialViewResult Sizes()
        {
            var items = _imageProcessor.GetImageSizes();
            return PartialView(items);
        }

        public PartialViewResult ImagePreview(int id, string size)
        {
            var mediaFile = _fileService.GetFile(id);
            var imageSize = _imageProcessor.GetImageSizes().Find(imgSize => imgSize.Name == size);

            return imageSize.Size == Size.Empty
                       ? PartialView((object)mediaFile.FileLocation)
                       : PartialView((object)_fileService.GetFileLocation(mediaFile, imageSize));
        }

        public string GetFileUrl(string value)
        {
            var mediaFile = _fileService.GetFileByLocation(value);
            if (mediaFile != null)
            {
                return "/" + mediaFile.FileLocation;
            }

            var split = value.Split('-');
            var id = Convert.ToInt32(split[0]);
            var file = _fileService.GetFile(id);
            var imageSize = file.Sizes.FirstOrDefault(size => size.Size == new Size(Convert.ToInt32(split[1]), Convert.ToInt32(split[2])));
            return "/" + _fileService.GetFileLocation(file, imageSize);
        }

        public PartialViewResult MiniUploader(int id)
        {
            return PartialView(id);
        }

        public PartialViewResult FileResult(int id)
        {
            var mediaFile = _fileService.GetFile(id);
            return PartialView(mediaFile);
        }

        public PartialViewResult RemoveMedia()
        {
            return PartialView();
        }
    }
}