using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public class UpdateTagsService : IUpdateTagsService
    {
        private readonly IDocumentTagsUpdateService _documentTagsUpdateService;

        public UpdateTagsService(IDocumentTagsUpdateService documentTagsUpdateService)
        {
            _documentTagsUpdateService = documentTagsUpdateService;
        }

        public async Task SetTags(DocumentImportDTO documentDto, Webpage webpage)
        {
            var tags = documentDto.Tags;
            await _documentTagsUpdateService.SetTags(tags, webpage);
        }
    }
}