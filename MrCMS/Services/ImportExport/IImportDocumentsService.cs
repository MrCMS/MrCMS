using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IImportDocumentsService
    {
        //void ImportDocumentsFromDTOs(IEnumerable<DocumentImportDTO> items);
        //Webpage ImportDocument(DocumentImportDTO dto);
        void CreateBatch(List<DocumentImportDTO> items);
    }
}