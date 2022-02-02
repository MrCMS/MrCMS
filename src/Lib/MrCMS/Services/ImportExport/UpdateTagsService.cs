using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public class UpdateTagsService : IUpdateTagsService
    {
        private readonly IWebpageTagsUpdateService _webpageTagsUpdateService;

        public UpdateTagsService(IWebpageTagsUpdateService webpageTagsUpdateService)
        {
            _webpageTagsUpdateService = webpageTagsUpdateService;
        }

        public async Task SetTags(DocumentImportDTO documentDto, Webpage webpage)
        {
            var tags = documentDto.Tags;
            await _webpageTagsUpdateService.SetTags(tags, webpage);
        }
    }
}