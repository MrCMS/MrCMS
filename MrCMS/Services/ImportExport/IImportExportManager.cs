using System.Collections.Generic;
using System.IO;
using MrCMS.Models;

namespace MrCMS.Services.ImportExport
{
    public interface IImportExportManager
    {
        byte[] ExportDocumentsToExcel();
        ImportDocumentsResult ImportDocumentsFromExcel(Stream file, bool autoStart = true);
        ExportDocumentsResult ExportDocumentsToEmail(ExportDocumentsModel model);
    }
}