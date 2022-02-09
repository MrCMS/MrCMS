using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MrCMS.Data;
using MrCMS.Entities.Documents.Media;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Services
{
    public class SaveFormFileUpload : ISaveFormFileUpload
    {
        private readonly IFileService _fileService;
        private readonly IGetMediaCategoryByPath _mediaCategoryLoader;
        private readonly IRepository<MediaCategory> _mediaCategoryRepository;

        public SaveFormFileUpload(IGetMediaCategoryByPath mediaCategoryLoader,
            IRepository<MediaCategory> mediaCategoryRepository, IFileService fileService)
        {
            _mediaCategoryLoader = mediaCategoryLoader;
            _mediaCategoryRepository = mediaCategoryRepository;
            _fileService = fileService;
        }

        public async Task<string> SaveFile(Form form, FormPosting formPosting, IFormFile file)
        {
            var mediaCategory = await _mediaCategoryLoader.GetByPath("file-uploads") ??
                                await CreateFileUploadMediaCategory();

            var result = await _fileService.AddFile(file.OpenReadStream(),
                form.Id + "-" + formPosting.Id + "-" + file.FileName,
                file.ContentType, file.Length, mediaCategory);

            return result.FileUrl;
        }

        private async Task<MediaCategory> CreateFileUploadMediaCategory()
        {
            var mediaCategory = new MediaCategory {Path = "file-uploads", Name = "File Uploads"};
            await _mediaCategoryRepository.Add(mediaCategory);
            return mediaCategory;
        }
    }
}