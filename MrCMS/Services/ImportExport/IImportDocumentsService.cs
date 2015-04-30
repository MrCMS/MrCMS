using System.Collections.Generic;
using MrCMS.Batching.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services.ImportExport.DTOs;

namespace MrCMS.Services.ImportExport
{
    public interface IImportDocumentsService
    {
        //void ImportDocumentsFromDTOs(IEnumerable<DocumentImportDTO> items);
        //Webpage ImportDocument(DocumentImportDTO dto);
        Batch CreateBatch(List<DocumentImportDTO> items, bool autoStart = true);
    }
}