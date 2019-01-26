using System;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class SaveFormFileUpload : ISaveFormFileUpload
    {
        private readonly IFileService _fileService;
        private readonly IGetDocumentByUrl<MediaCategory> _mediaCategoryLoader;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;

        public SaveFormFileUpload(IGetDocumentByUrl<MediaCategory> mediaCategoryLoader,
            IRepository<MediaCategory> mediaCategoryRepository, IFileService fileService)
        {
            _mediaCategoryLoader = mediaCategoryLoader;
            _mediaCategoryRepository = mediaCategoryRepository;
            _fileService = fileService;
        }

        public string SaveFile(Form form, FormPosting formPosting, IFormFile file)
        {
            var mediaCategory = _mediaCategoryLoader.GetByUrl("file-uploads") ??
                                CreateFileUploadMediaCategory();

            var result = _fileService.AddFile(file.OpenReadStream(), form.Id + "-" + formPosting.Id + "-" + file.FileName,
                file.ContentType, file.Length, mediaCategory);

            return result.FileUrl;
        }

        private MediaCategory CreateFileUploadMediaCategory()
        {
            var mediaCategory = new MediaCategory { UrlSegment = "file-uploads", Name = "File Uploads" };
            _mediaCategoryRepository.Add(mediaCategory);
            return mediaCategory;
        }
    }
}