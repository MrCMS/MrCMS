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

        public void SetTags(DocumentImportDTO documentDto, Webpage webpage)
        {
            var tags = documentDto.Tags;
            _documentTagsUpdateService.SetTags(tags, webpage);
        }
    }
}