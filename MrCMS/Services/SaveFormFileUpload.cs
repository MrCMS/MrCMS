using System.Web;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class SaveFormFileUpload : ISaveFormFileUpload
    {
        private readonly IGetDocumentByUrl<MediaCategory> _mediaCategoryLoader;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;
        private readonly IFileService _fileService;

        public SaveFormFileUpload(IGetDocumentByUrl<MediaCategory> mediaCategoryLoader, IRepository<MediaCategory> mediaCategoryRepository, IFileService fileService)
        {
            _mediaCategoryLoader = mediaCategoryLoader;
            _mediaCategoryRepository = mediaCategoryRepository;
            _fileService = fileService;
        }

        private MediaCategory CreateFileUploadMediaCategory()
        {
            var mediaCategory = new MediaCategory { UrlSegment = "file-uploads", Name = "File Uploads" };
            _mediaCategoryRepository.Add(mediaCategory);
            return mediaCategory;
        }
        public string SaveFile(Webpage webpage, FormPosting formPosting, HttpPostedFileBase file)
        {
            var mediaCategory = _mediaCategoryLoader.GetByUrl("file-uploads") ??
                                CreateFileUploadMediaCategory();

            var result = _fileService.AddFile(file.InputStream, webpage.Id + "-" + formPosting.Id + "-" + file.FileName, file.ContentType, file.ContentLength, mediaCategory);

            return result.FileUrl;
        }
    }
}