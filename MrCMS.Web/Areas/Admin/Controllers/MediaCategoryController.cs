using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Services;
using MrCMS.Website.Binders;
using System.Linq;
using MrCMS.Helpers;

namespace MrCMS.Web.Areas.Admin.Controllers
{
    public class MediaCategoryController : BaseDocumentController<MediaCategory>
    {
        private readonly IFileService _fileService;

        public MediaCategoryController(IDocumentService documentService, IFileService fileService)
            : base(documentService)
        {
            _fileService = fileService;
        }

        /**
         * Need to do media category specific stuff before generic stuff. In this case
         * create a directory for media files.
         */

        public override ActionResult Add([IoCModelBinder(typeof(AddDocumentModelBinder))] MediaCategory doc)
        {
            var actionResult = base.Add(doc);
            _fileService.CreateFolder(doc);
            return actionResult;
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

        public PartialViewResult MediaSelector(int? categoryId, bool imagesOnly = false, int page = 1)
        {
            ViewData["categories"] = _documentService.GetAllDocuments<MediaCategory>().OrderBy(category => category.Name).BuildSelectItemList
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
    }
}