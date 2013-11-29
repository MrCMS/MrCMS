using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IUpdateTagsService
    {
        IUpdateTagsService Inititalise();
        void SetTags(DocumentImportDTO documentDto, Webpage webpage);
        void SaveTags();
    }
}