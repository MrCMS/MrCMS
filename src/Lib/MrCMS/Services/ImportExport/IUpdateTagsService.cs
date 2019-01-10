using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IUpdateTagsService
    {
        void SetTags(DocumentImportDTO documentDto, Webpage webpage);
    }
}